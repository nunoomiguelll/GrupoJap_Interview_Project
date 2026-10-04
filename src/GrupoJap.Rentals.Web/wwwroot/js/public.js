// Comportamentos exclusivos do site público: carrossel e navegação por âncoras.
const reducedMotion = window.matchMedia("(prefers-reduced-motion: reduce)");
const scrollBehavior = () => (reducedMotion.matches ? "auto" : "smooth");

// ---------- Carrossel ----------
// Mostra N cartões por página (--per-view no CSS) e avança uma página a cada data-interval ms (5 s por omissão).
// Pausa com o rato por cima, com foco no interior ou com o separador escondido.
for (const carousel of document.querySelectorAll("[data-carousel]")) {
	const track = carousel.querySelector("[data-carousel-track]");
	const slides = [...track.children];
	const interval = Number(carousel.dataset.interval) || 5000;
	let page = 0;
	let timer = null;
	let hovering = false;
	let focused = false;

	// Textos traduzidos vêm de data-label-* (definidos na vista).
	const labels = carousel.dataset;
	const makeArrow = (symbol, label) => {
		const button = document.createElement("button");
		button.type = "button";
		button.className = "carousel-arrow";
		button.textContent = symbol;
		button.setAttribute("aria-label", label);
		return button;
	};
	const prevButton = makeArrow("←", labels.labelPrev || "Página anterior");
	const nextButton = makeArrow("→", labels.labelNext || "Página seguinte");
	const dots = document.createElement("div");
	dots.className = "carousel-dots";
	const controls = document.createElement("div");
	controls.className = "carousel-controls";
	controls.append(prevButton, dots, nextButton);
	carousel.append(controls);
	carousel.style.setProperty("--interval", `${interval}ms`);
	carousel.classList.add("is-ready");

	const perView = () => Math.max(1, parseInt(getComputedStyle(carousel).getPropertyValue("--per-view"), 10) || 1);
	const pageCount = () => Math.ceil(slides.length / perView());

	function render() {
		const count = pageCount();
		page = Math.min(page, count - 1);
		const gap = getComputedStyle(track).columnGap || "0px";
		track.style.transform = `translateX(calc(${-page} * (100% + ${gap})))`;

		const first = page * perView();
		slides.forEach((slide, index) => {
			const visible = index >= first && index < first + perView();
			slide.toggleAttribute("inert", !visible);
			slide.setAttribute("aria-hidden", String(!visible));
		});

		controls.hidden = count <= 1;
		if (dots.children.length !== count) {
			dots.replaceChildren(...Array.from({ length: count }, (_, index) => {
				const dot = document.createElement("button");
				dot.type = "button";
				dot.className = "carousel-dot";
				dot.setAttribute("aria-label", (labels.labelGoto || "Ir para a página {0} de {1}").replace("{0}", index + 1).replace("{1}", count));
				dot.addEventListener("click", () => goTo(index));
				return dot;
			}));
		}
		[...dots.children].forEach((dot, index) => {
			dot.classList.toggle("is-active", index === page);
			dot.setAttribute("aria-current", index === page ? "true" : "false");
		});
		resetProgress();
	}

	// Reinicia a barra de progresso do ponto ativo, para coincidir com o temporizador.
	function resetProgress() {
		const dot = dots.children[page];
		if (!dot) return;
		dot.classList.remove("is-active");
		void dot.offsetWidth;
		dot.classList.add("is-active");
	}

	function goTo(target) {
		const count = pageCount();
		page = (target + count) % count;
		render();
		restart();
	}

	function restart() {
		clearInterval(timer);
		timer = null;
		const paused = hovering || focused || document.hidden || pageCount() <= 1;
		carousel.classList.toggle("is-paused", paused);
		if (!paused) {
			resetProgress();
			timer = setInterval(() => {
				page = (page + 1) % pageCount();
				render();
			}, interval);
		}
	}

	prevButton.addEventListener("click", () => goTo(page - 1));
	nextButton.addEventListener("click", () => goTo(page + 1));
	carousel.addEventListener("mouseenter", () => { hovering = true; restart(); });
	carousel.addEventListener("mouseleave", () => { hovering = false; restart(); });
	carousel.addEventListener("focusin", () => { focused = true; restart(); });
	carousel.addEventListener("focusout", (event) => {
		if (carousel.contains(event.relatedTarget)) return;
		focused = false;
		restart();
	});
	document.addEventListener("visibilitychange", restart);

	let lastPerView = perView();
	window.addEventListener("resize", () => {
		if (perView() === lastPerView) return;
		lastPerView = perView();
		render();
		restart();
	});

	render();
	restart();
}

// ---------- Navegação por âncoras ----------
// Um link para a página atual (ex.: "Início" estando na página inicial) faz scroll suave
// para o topo ou para a secção indicada no #hash, em vez de recarregar a página.
const header = document.querySelector(".site-header");

function scrollToTarget(hash) {
	const target = hash ? document.getElementById(decodeURIComponent(hash.slice(1))) : null;
	if (hash && !target) return false;
	const offset = (header?.offsetHeight ?? 0) + 16;
	const top = target ? target.getBoundingClientRect().top + window.scrollY - offset : 0;
	window.scrollTo({ top: Math.max(0, top), behavior: scrollBehavior() });
	return true;
}

document.addEventListener("click", (event) => {
	const link = event.target.closest(".site-header a[href], .site-footer a[href]");
	if (!link || event.defaultPrevented || event.button !== 0 || event.metaKey || event.ctrlKey || event.shiftKey || event.altKey || link.target === "_blank") return;

	const url = new URL(link.href, location.href);
	const samePage = url.origin === location.origin && url.pathname === location.pathname && url.search === location.search;
	if (!samePage || !scrollToTarget(url.hash)) return;

	event.preventDefault();
	history.pushState(null, "", url.hash || url.pathname + url.search);
	document.body.classList.remove("site-menu-open");
	document.querySelector("[data-site-menu]")?.setAttribute("aria-expanded", "false");
	updateCurrentLink();
});

// ---------- Item ativo da navbar na página inicial ----------
// "Como funciona" e "Contactos" são secções da página inicial: marca o item cuja secção está visível.
const navLinks = [...document.querySelectorAll(".site-nav a[href]")];
const sectionLinks = navLinks
	.map((link) => {
		const url = new URL(link.href, location.href);
		const section = url.pathname === location.pathname && url.hash ? document.getElementById(url.hash.slice(1)) : null;
		return section ? { link, section } : null;
	})
	.filter(Boolean);
const pageLinks = navLinks.filter((link) => link.classList.contains("is-current"));

function updateCurrentLink() {
	if (sectionLinks.length === 0) return;
	const line = (header?.offsetHeight ?? 0) + window.innerHeight * 0.35;
	const atBottom = window.innerHeight + window.scrollY >= document.documentElement.scrollHeight - 4;
	let active = null;
	for (const entry of sectionLinks) {
		if (entry.section.getBoundingClientRect().top <= line) active = entry;
	}
	if (atBottom) active = sectionLinks.at(-1);

	for (const { link } of sectionLinks) link.classList.toggle("is-current", link === active?.link);
	for (const link of pageLinks) link.classList.toggle("is-current", !active);
}

if (sectionLinks.length > 0) {
	let ticking = false;
	window.addEventListener("scroll", () => {
		if (ticking) return;
		ticking = true;
		requestAnimationFrame(() => { ticking = false; updateCurrentLink(); });
	}, { passive: true });
	updateCurrentLink();
}

// ---------- Seletor de idioma ----------
// Fecha ao clicar fora ou com Escape (o menu de conta já é tratado em site.js).
document.addEventListener("click", (event) => {
	for (const menu of document.querySelectorAll(".lang-menu[open]")) {
		if (!menu.contains(event.target)) menu.removeAttribute("open");
	}
});
document.addEventListener("keydown", (event) => {
	if (event.key === "Escape") document.querySelectorAll(".lang-menu[open]").forEach((menu) => menu.removeAttribute("open"));
});
