document.addEventListener("DOMContentLoaded", () => {

    /* ─────────  Tabs & slider  ───────── */
    const slider = document.querySelector(".tab-slider");
    document.querySelectorAll(".auth-tab").forEach((tab, i) => {
        tab.addEventListener("click", e => {
            /* allow navigation (server-side) when href is different page */
            if (tab.classList.contains("active")) e.preventDefault();
            slider.style.transform = `translateX(${i * 100}%)`;
        });
    });

    /* ─────────  Ripple for buttons  ───────── */
    document.querySelectorAll(".btn-animate")
        .forEach(btn => btn.addEventListener("click", e => {
            const r = document.createElement("span");
            r.className = "ripple"; btn.appendChild(r);
            const d = Math.max(btn.clientWidth, btn.clientHeight);
            r.style.width = r.style.height = d + "px";
            r.style.left = e.clientX - btn.getBoundingClientRect().left - d / 2 + "px";
            r.style.top = e.clientY - btn.getBoundingClientRect().top - d / 2 + "px";
            setTimeout(() => r.remove(), 600);
        }));

    /* ─────────  Dynamic validation (Register)  ───────── */
    const reg = document.querySelector("form[action$='Register']");
    if (!reg) return;               // on Login page we’re done

    /* field handles */
    const first = reg.querySelector("[asp-for='FirstName']");
    const last = reg.querySelector("[asp-for='LastName']");
    const email = reg.querySelector("[asp-for='Email']");
    const pass = reg.querySelector("#registerPassword");
    const confirm = reg.querySelector("[asp-for='ConfirmPassword']");
    const submit = reg.querySelector("button[type='submit']");

    /* rule elements */
    const ruleEls = [...document.querySelectorAll(".rule-list li")];

    /* regexes */
    const rxEmail = /^[\w.!#$%&’*+/=?^`{|}~-]+@[\w-]+(\.[\w-]+)+$/i;
    const rules = [
        v => v.length >= 8,
        v => /[A-Z]/.test(v),
        v => /[a-z]/.test(v),
        v => /\d/.test(v)
    ];

    /* util */
    const setState = (el, ok) => {
        el.classList.toggle("is-valid", ok);
        el.classList.toggle("is-invalid", !ok);
    };

    /* live password strength */
    pass.addEventListener("input", () => {
        const v = pass.value;
        let passed = 0;
        ruleEls.forEach((li, i) => {
            const ok = rules[i](v);
            li.classList.toggle("rule-ok", ok);
            li.classList.toggle("rule-bad", !ok);
            li.querySelector("i").className = ok
                ? "fas fa-check-circle"
                : "fas fa-times-circle";
            if (ok) passed++;
        });

        const bar = document.querySelector("#strengthBar span");
        bar.style.width = `${passed * 25}%`;
        bar.style.background = ["#e74", "#f2b72b", "#e3c600", "#27a844"][passed] || "#e74";
        validateForm();
    });

    /* simple field listeners */
    [first, last, email, confirm, pass].forEach(inp =>
        inp.addEventListener("input", validateForm));

    /* on submit, block if invalid (extra safety) */
    reg.addEventListener("submit", e => {
        if (!validateForm()) e.preventDefault();
    });

    function validateForm() {
        const okFirst = first.value.trim().length > 1;
        const okLast = last.value.trim().length > 1;
        const okEmail = rxEmail.test(email.value);
        const okPass = rules.every(r => r(pass.value));
        const okConfirm = confirm.value === pass.value && okPass;

        setState(first, okFirst);
        setState(last, okLast);
        setState(email, okEmail);
        setState(pass, okPass);
        setState(confirm, okConfirm);

        const allGood = okFirst && okLast && okEmail && okPass && okConfirm;
        submit.disabled = !allGood;
        return allGood;
    }
});
