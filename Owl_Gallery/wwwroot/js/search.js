document.addEventListener('DOMContentLoaded', () => {
    const input = document.getElementById('searchInput');
    const dropdown = document.getElementById('searchDropdown');
    const liveResults = document.getElementById('liveResults');
    const trending = document.getElementById('trendingCategories');
    let timeout = null;

    if (!input || !dropdown || !liveResults) {
        console.error("Search input, dropdown, or liveResults not found.");
        return;
    }

    const trendingHTML = `
        <a href="/Products/Products?category=Necklaces" class="btn btn-outline-secondary btn-sm rounded-pill">Necklaces</a>
        <a href="/Products/Products?category=Earrings" class="btn btn-outline-secondary btn-sm rounded-pill">Earrings</a>
        <a href="/Products/Products?category=Bracelets" class="btn btn-outline-secondary btn-sm rounded-pill">Bracelets</a>
        <a href="/Products/Products?category=Rings" class="btn btn-outline-secondary btn-sm rounded-pill">Rings</a>
    `;

    // When focusing the input
    input.addEventListener('focus', () => {
        dropdown.style.display = 'block';
        trending.innerHTML = trendingHTML;
        liveResults.innerHTML = `<div class="text-center text-muted">Start typing to see results...</div>`;
    });

    // On typing input
    input.addEventListener('input', () => {
        clearTimeout(timeout);
        const query = input.value.trim();

        if (!query) {
            liveResults.innerHTML = `<div class="text-center text-muted">Start typing to see results...</div>`;
            return;
        }

        liveResults.innerHTML = `<div class="text-center">
            <div class="spinner-border text-purple" role="status"></div>
        </div>`;

        timeout = setTimeout(async () => {
            try {
                const response = await fetch(`/Products/SearchLive?query=${encodeURIComponent(query)}`);
                const data = await response.json();

                if (data.length === 0) {
                    liveResults.innerHTML = `<div class="text-center text-muted">No results found.</div>`;
                    return;
                }

                // Build beautiful live results
                liveResults.innerHTML = data.map(p => `
                    <a href="/Products/Details/${p.id}" class="list-group-item list-group-item-action d-flex align-items-center gap-3">
                        <img src="${p.imageUrl}" alt="${p.name}" style="width:40px;height:40px;object-fit:cover;border-radius:8px;">
                        <div>
                            <div><strong>${highlightMatch(p.name, query)}</strong></div>
                            <small class="text-muted">${highlightMatch(p.category, query)}</small>
                        </div>
                    </a>
                `).join('');
            } catch (err) {
                console.error(err);
                liveResults.innerHTML = `<div class="text-danger text-center">Error loading results</div>`;
            }
        }, 300);
    });

    // Hide dropdown when clicking outside
    document.addEventListener('click', (e) => {
        if (!dropdown.contains(e.target) && e.target !== input) {
            dropdown.style.display = 'none';
        }
    });

    // Highlight matched part of text
    function highlightMatch(text, query) {
        const regex = new RegExp(`(${query})`, "gi");
        return text.replace(regex, `<mark>$1</mark>`);
    }
});
