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
	const locale = document.documentElement.lang || undefined;
	const searchTerm = rentalSearch.value.trim().toLocaleLowerCase(locale);
	let visibleCount = 0;

	for (const row of rentalRows) {
		const matches = row.textContent.toLocaleLowerCase(locale).includes(searchTerm);
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
	themeToggle?.setAttribute("aria-label", (theme === "dark" ? themeToggle.dataset.labelLight : themeToggle.dataset.labelDark) || "");
	document.querySelector('meta[name="theme-color"]')?.setAttribute("content", theme === "dark" ? "#151011" : "#f6f3f1");
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
		preview.alt = photoInput.dataset.previewAlt || "";
		preview.setAttribute("data-photo-preview", "");
		fallback?.replaceWith(preview);
	}
	preview.src = url;
});

// Fecha o seletor de idioma ao clicar fora ou com Escape.
document.addEventListener("click", (event) => {
	for (const menu of document.querySelectorAll(".lang-menu[open]")) {
		if (!menu.contains(event.target)) menu.removeAttribute("open");
	}
});
document.addEventListener("keydown", (event) => {
	if (event.key !== "Escape") return;
	document.querySelectorAll(".lang-menu[open]").forEach((menu) => menu.removeAttribute("open"));
});
