(function () {
    const storedTheme = localStorage.getItem("theme");
    const prefersDark = window.matchMedia("(prefers-color-scheme: dark)").matches;

    const theme = storedTheme || (prefersDark ? "dark" : "light");
    document.documentElement.setAttribute("data-bs-theme", theme);

    const toggle = document.getElementById("themeToggle");
    if (!toggle) return;

    toggle.textContent = theme === "dark" ? "☀️" : "🌙";

    toggle.addEventListener("click", () => {
        const current = document.documentElement.getAttribute("data-bs-theme");
        const next = current === "dark" ? "light" : "dark";

        document.documentElement.setAttribute("data-bs-theme", next);
        localStorage.setItem("theme", next);

        toggle.textContent = next === "dark" ? "☀️" : "🌙";
    });
})();
