// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
const sidebarToggle = document.querySelector("[data-sidebar-toggle]");
const sidebarClose = document.querySelector("[data-sidebar-close]");

function setSidebarOpen(isOpen) {
	document.body.classList.toggle("sidebar-open", isOpen);
	sidebarToggle?.setAttribute("aria-expanded", String(isOpen));
}

sidebarToggle?.addEventListener("click", () => {
	setSidebarOpen(!document.body.classList.contains("sidebar-open"));
});

sidebarClose?.addEventListener("click", () => setSidebarOpen(false));

document.addEventListener("keydown", (event) => {
	if (event.key === "Escape") setSidebarOpen(false);
});

const rentalSearch = document.querySelector("[data-rental-search]");
const rentalRows = [...document.querySelectorAll("[data-rental-row]")];
const noSearchResults = document.querySelector("[data-no-results]");

rentalSearch?.addEventListener("input", () => {
	const searchTerm = rentalSearch.value.trim().toLocaleLowerCase("pt-PT");
	let visibleCount = 0;

	for (const row of rentalRows) {
		const matches = row.textContent.toLocaleLowerCase("pt-PT").includes(searchTerm);
		row.hidden = !matches;
		if (matches) visibleCount += 1;
	}

	if (noSearchResults) noSearchResults.hidden = visibleCount > 0;
});

// Modo noturno: a escolha fica guardada no browser; sem escolha, segue o tema do sistema.
const themeToggle = document.querySelector("[data-theme-toggle]");

function applyTheme(theme) {
	document.documentElement.dataset.theme = theme;
	themeToggle?.setAttribute("aria-pressed", String(theme === "dark"));
	themeToggle?.setAttribute("aria-label", theme === "dark" ? "Ativar modo claro" : "Ativar modo noturno");
	document.querySelector('meta[name="theme-color"]')?.setAttribute("content", theme === "dark" ? "#111613" : "#f4f5f0");
}

applyTheme(document.documentElement.dataset.theme === "dark" ? "dark" : "light");

themeToggle?.addEventListener("click", () => {
	const next = document.documentElement.dataset.theme === "dark" ? "light" : "dark";
	applyTheme(next);
	try { localStorage.setItem("theme", next); } catch { /* armazenamento indisponível */ }
});

// Pre-visualizacao da foto de perfil antes de guardar.
const photoInput = document.querySelector("[data-photo-input]");
photoInput?.addEventListener("change", () => {
	const file = photoInput.files?.[0];
	if (!file || !file.type.startsWith("image/")) return;
	const url = URL.createObjectURL(file);
	let preview = document.querySelector("[data-photo-preview]");
	if (!preview) {
		const fallback = document.querySelector("[data-photo-preview-fallback]");
		preview = document.createElement("img");
		preview.className = "profile-photo";
		preview.alt = "Pré-visualização da foto";
		preview.setAttribute("data-photo-preview", "");
		fallback?.replaceWith(preview);
	}
	preview.src = url;
});

// ---------- Site publico ----------
const siteMenuButton = document.querySelector("[data-site-menu]");
siteMenuButton?.addEventListener("click", () => {
	const open = !document.body.classList.contains("site-menu-open");
	document.body.classList.toggle("site-menu-open", open);
	siteMenuButton.setAttribute("aria-expanded", String(open));
});

// Fecha o menu de conta ao clicar fora ou com Escape.
document.addEventListener("click", (event) => {
	for (const menu of document.querySelectorAll(".account-menu[open]")) {
		if (!menu.contains(event.target)) menu.removeAttribute("open");
	}
});
document.addEventListener("keydown", (event) => {
	if (event.key !== "Escape") return;
	document.querySelectorAll(".account-menu[open]").forEach((menu) => menu.removeAttribute("open"));
	document.body.classList.remove("site-menu-open");
});

// Datas ligadas: a devolucao tem de ser depois do levantamento; mostra o numero de dias.
function addDays(isoDate, days) {
	const date = new Date(isoDate + "T00:00:00");
	date.setDate(date.getDate() + days);
	return date.toISOString().slice(0, 10);
}

for (const form of document.querySelectorAll("[data-date-range]")) {
	const startInput = form.querySelector("[data-range-start]");
	const endInput = form.querySelector("[data-range-end]");
	const daysLabel = form.querySelector("[data-range-days]");
	if (!startInput || !endInput) continue;

	const sync = () => {
		if (startInput.value) {
			const minEnd = addDays(startInput.value, 1);
			endInput.min = minEnd;
			if (endInput.value && endInput.value < minEnd) endInput.value = minEnd;
		}
		if (daysLabel) {
			if (startInput.value && endInput.value && endInput.value > startInput.value) {
				const days = Math.round((new Date(endInput.value) - new Date(startInput.value)) / 86400000) + 1;
				daysLabel.textContent = (daysLabel.dataset.template || "{0} dia(s) de aluguer").replace("{0}", days);
				daysLabel.hidden = false;
			} else {
				daysLabel.hidden = true;
			}
		}
	};

	startInput.addEventListener("change", sync);
	endInput.addEventListener("change", sync);
	sync();
}

// Confirmacao antes de acoes destrutivas (ex.: cancelar reserva).
for (const form of document.querySelectorAll("form[data-confirm]")) {
	form.addEventListener("submit", (event) => {
		if (!window.confirm(form.dataset.confirm)) event.preventDefault();
	});
}

// Video do hero: so aparece depois de comecar a reproduzir; pausa quando fora do ecra ou com movimento reduzido.
const heroVideo = document.querySelector("[data-hero-video]");
if (heroVideo) {
	const reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)");
	const tryPlay = () => {
		if (reduceMotion.matches) return;
		heroVideo.play().then(() => heroVideo.classList.add("is-playing")).catch(() => { /* autoplay bloqueado: fica o fundo em gradiente */ });
	};
	heroVideo.addEventListener("playing", () => heroVideo.classList.add("is-playing"));
	if ("IntersectionObserver" in window) {
		new IntersectionObserver(([entry]) => {
			if (entry.isIntersecting) tryPlay(); else heroVideo.pause();
		}).observe(heroVideo);
	} else {
		tryPlay();
	}
}
