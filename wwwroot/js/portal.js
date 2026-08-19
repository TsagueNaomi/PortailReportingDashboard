/**
 * SOCADEL Reporting Portal - Master JavaScript
 * Handles navigation tree, enhanced search with highlighting & instant dropdown,
 * workspace actions, chart micro-interactions, and admin features.
 */

document.addEventListener('DOMContentLoaded', function () {
    initTreeNavigation();
    initSearchFilter();
    initActionButtons();
    initChartInteractions();
    initAdminFeatures();
});

/* ==========================================================================
   1. Tree Navigation (3 Levels)
   ========================================================================== */
function initTreeNavigation() {
    // Level 1 Category Toggles (Commercial, Finance)
    document.querySelectorAll('.tree-cat-header').forEach(header => {
        header.addEventListener('click', function (e) {
            e.preventDefault();
            const parentItem = this.closest('.tree-level-1-item');
            if (parentItem) {
                parentItem.classList.toggle('collapsed');
            }
        });
    });

    // Level 2 Subcategory Toggles (Encaissement, Facturation, Recouvrement, etc.)
    document.querySelectorAll('.tree-sub-header').forEach(header => {
        header.addEventListener('click', function (e) {
            e.preventDefault();
            const parentItem = this.closest('.tree-level-2-item');
            if (parentItem) {
                parentItem.classList.toggle('collapsed');
            }
        });
    });

    // Ensure active item and all its ancestors are expanded on load
    const activeReport = document.querySelector('.tree-report-link.active');
    if (activeReport) {
        let parent = activeReport.closest('.tree-level-2-item');
        if (parent) parent.classList.remove('collapsed');
        let grandParent = activeReport.closest('.tree-level-1-item');
        if (grandParent) grandParent.classList.remove('collapsed');
    }

    // Level 3 Report Links: Smooth iframe update without full page reload & active state transition
    document.querySelectorAll('.tree-report-link').forEach(link => {
        link.addEventListener('click', function (e) {
            const reportId = this.getAttribute('data-id');
            const pbiContainer = document.querySelector('.pbi-report-container');

            if (pbiContainer && reportId) {
                e.preventDefault();

                // 1. Update Active class in sidebar menu
                document.querySelectorAll('.tree-report-link').forEach(r => r.classList.remove('active'));
                this.classList.add('active');

                // 2. Expand parent categories
                let parentSub = this.closest('.tree-level-2-item');
                if (parentSub) parentSub.classList.remove('collapsed');
                let parentCat = this.closest('.tree-level-1-item');
                if (parentCat) parentCat.classList.remove('collapsed');

                // 3. Update Browser URL smoothly
                window.history.pushState({ reportId }, '', `/?report=${encodeURIComponent(reportId)}`);

                // 4. Update Header Titles dynamically
                const reportTitle = this.querySelector('.report-link-text')?.textContent.trim() || 'Rapport décisionnel';
                const topTitle = document.querySelector('.pbi-report-title-top');
                if (topTitle) topTitle.textContent = reportTitle;

                const currentBreadcrumb = document.querySelector('.breadcrumb-current-page');
                if (currentBreadcrumb) currentBreadcrumb.textContent = reportTitle;

                // 5. Update iframe source smoothly
                let reportUrl = this.getAttribute('data-url');
                if (!reportUrl || reportUrl === '') {
                    if (reportId === 'rep-enc-synth') {
                        reportUrl = "https://app.powerbi.com/view?r=eyJrIjoiM2ZmZGNmMjctZmU3OC00MzdjLTgyN2EtZWMzZWM4NTM1NjYwIiwidCI6IjY5OWFjZTY3LWQyZTQtNGJjZC1iMzAzLWQyYmJlMmI5YmJmMSJ9";
                    } else if (reportId === 'rep-enc-modes') {
                        reportUrl = "https://app.powerbi.com/view?r=eyJrIjoiMTY5ZTIyN2MtOGVmYy00NTc3LTkzMWMtZmNkMTZiNDc4NWJkIiwidCI6ImZjZTBkOTIyLWMzMjktNGMwMC04MTY3LTZkYzQ4ZTM3ZWEwNSJ9";
                    }
                }

                const iframe = document.querySelector('.pbi-iframe-wrapper iframe');
                if (iframe && reportUrl) {
                    iframe.style.opacity = '0.3';
                    iframe.style.transition = 'opacity 0.25s ease';
                    iframe.src = reportUrl;
                    setTimeout(() => {
                        iframe.style.opacity = '1';
                    }, 300);
                } else {
                    window.location.href = `/?report=${encodeURIComponent(reportId)}`;
                }
            }
        });
    });
}

/* ==========================================================================
   2. Enhanced Search Filtering (Sidebar & Header Live Search)
   ========================================================================== */
function initSearchFilter() {
    const sidebarInput = document.getElementById('sidebarSearchInput');
    const sidebarClear = document.getElementById('sidebarSearchClear');
    const sidebarBadge = document.getElementById('sidebarSearchBadge');

    const headerInput = document.getElementById('headerSearchInput');
    const headerClear = document.getElementById('headerSearchClear');
    const headerResults = document.getElementById('headerSearchResults');

    function performTreeSearch(query) {
        const term = (query || '').trim().toLowerCase();
        const reportItems = document.querySelectorAll('.tree-level-3-item');
        const subItems = document.querySelectorAll('.tree-level-2-item');
        const catItems = document.querySelectorAll('.tree-level-1-item');

        if (sidebarClear) sidebarClear.style.display = term ? 'flex' : 'none';
        if (headerClear) headerClear.style.display = term ? 'flex' : 'none';

        if (!term) {
            // Reset tree view
            if (sidebarBadge) sidebarBadge.style.display = 'none';

            reportItems.forEach(item => {
                item.style.display = '';
                const link = item.querySelector('.report-link-text');
                if (link && link.getAttribute('data-original')) {
                    link.innerHTML = link.getAttribute('data-original');
                }
            });

            subItems.forEach(item => {
                item.style.display = '';
            });

            catItems.forEach(item => {
                item.style.display = '';
            });

            return;
        }

        let matchCount = 0;

        reportItems.forEach(item => {
            const link = item.querySelector('.tree-report-link');
            const textSpan = item.querySelector('.report-link-text');
            if (!link || !textSpan) return;

            if (!textSpan.hasAttribute('data-original')) {
                textSpan.setAttribute('data-original', textSpan.textContent);
            }

            const rawText = textSpan.getAttribute('data-original') || textSpan.textContent;
            const fullDataTitle = (item.getAttribute('data-title') || rawText).toLowerCase();

            if (fullDataTitle.includes(term)) {
                matchCount++;
                item.style.display = '';

                // Highlight matched portion
                const regex = new RegExp(`(${escapeRegex(term)})`, 'gi');
                textSpan.innerHTML = rawText.replace(regex, '<mark class="search-highlight">$1</mark>');

                // Auto-expand and reveal parents
                let sub = item.closest('.tree-level-2-item');
                if (sub) {
                    sub.classList.remove('collapsed');
                    sub.style.display = '';
                }

                let cat = item.closest('.tree-level-1-item');
                if (cat) {
                    cat.classList.remove('collapsed');
                    cat.style.display = '';
                }
            } else {
                item.style.display = 'none';
                textSpan.innerHTML = rawText;
            }
        });

        // Hide empty subcategories and categories
        subItems.forEach(sub => {
            const hasVisibleReports = sub.querySelectorAll('.tree-level-3-item:not([style*="display: none"])').length > 0;
            if (!hasVisibleReports) {
                sub.style.display = 'none';
            }
        });

        catItems.forEach(cat => {
            const hasVisibleSubs = cat.querySelectorAll('.tree-level-2-item:not([style*="display: none"])').length > 0;
            if (!hasVisibleSubs) {
                cat.style.display = 'none';
            }
        });

        // Update badge
        if (sidebarBadge) {
            sidebarBadge.style.display = 'block';
            if (matchCount === 0) {
                sidebarBadge.textContent = 'Aucun résultat trouvé';
                sidebarBadge.style.backgroundColor = '#FFEBEE';
                sidebarBadge.style.color = '#C62828';
            } else {
                sidebarBadge.textContent = `${matchCount} rapport${matchCount > 1 ? 's' : ''} trouvé${matchCount > 1 ? 's' : ''}`;
                sidebarBadge.style.backgroundColor = 'var(--socadel-blue-light)';
                sidebarBadge.style.color = 'var(--socadel-blue-dark)';
            }
        }
    }

    function updateHeaderDropdown(query) {
        if (!headerResults) return;
        const term = (query || '').trim().toLowerCase();

        if (!term) {
            headerResults.style.display = 'none';
            headerResults.innerHTML = '';
            return;
        }

        const reportLinks = document.querySelectorAll('.tree-report-link');
        const matches = [];

        reportLinks.forEach(link => {
            const id = link.getAttribute('data-id');
            const dataTitle = link.getAttribute('data-title') || link.textContent.trim();
            const text = link.querySelector('.report-link-text')?.textContent.trim() || link.textContent.trim();
            
            const subTitle = link.closest('.tree-level-2-item')?.getAttribute('data-title') || '';
            const catTitle = link.closest('.tree-level-1-item')?.getAttribute('data-title') || '';

            if (dataTitle.toLowerCase().includes(term) || catTitle.toLowerCase().includes(term)) {
                matches.push({ id, title: text, path: `${catTitle} › ${subTitle}` });
            }
        });

        if (matches.length === 0) {
            headerResults.innerHTML = '<div class="search-result-empty">Aucun rapport correspondant</div>';
        } else {
            headerResults.innerHTML = matches.slice(0, 6).map(m => `
                <a href="/?report=${m.id}" class="search-result-item">
                    <span class="search-result-title">${escapeHtml(m.title)}</span>
                    <span class="search-result-path">${escapeHtml(m.path)}</span>
                </a>
            `).join('');
        }

        headerResults.style.display = 'block';
    }

    function escapeRegex(string) {
        return string.replace(/[-\/\\^$*+?.()|[\]{}]/g, '\\$&');
    }

    function escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    // Sidebar Search Listeners
    if (sidebarInput) {
        sidebarInput.addEventListener('input', function () {
            if (headerInput) headerInput.value = this.value;
            performTreeSearch(this.value);
        });
    }

    if (sidebarClear) {
        sidebarClear.addEventListener('click', function () {
            if (sidebarInput) sidebarInput.value = '';
            if (headerInput) headerInput.value = '';
            performTreeSearch('');
            if (sidebarInput) sidebarInput.focus();
        });
    }

    // Header Search Listeners
    if (headerInput) {
        headerInput.addEventListener('input', function () {
            if (sidebarInput) sidebarInput.value = this.value;
            performTreeSearch(this.value);
            updateHeaderDropdown(this.value);
        });

        headerInput.addEventListener('focus', function () {
            if (this.value.trim()) {
                updateHeaderDropdown(this.value);
            }
        });

        document.addEventListener('click', function (e) {
            if (headerResults && !headerResults.contains(e.target) && e.target !== headerInput) {
                headerResults.style.display = 'none';
            }
        });
    }

    if (headerClear) {
        headerClear.addEventListener('click', function () {
            if (headerInput) headerInput.value = '';
            if (sidebarInput) sidebarInput.value = '';
            performTreeSearch('');
            updateHeaderDropdown('');
            if (headerInput) headerInput.focus();
        });
    }
}

/* ==========================================================================
   3. Action Buttons (Fullscreen & Refresh)
   ========================================================================== */
function initActionButtons() {
    const fullscreenBtn = document.getElementById('btnFullscreen');
    const refreshBtn = document.getElementById('btnRefresh');
    const reportWorkspace = document.querySelector('.report-workspace');

    if (fullscreenBtn && reportWorkspace) {
        fullscreenBtn.addEventListener('click', function () {
            if (!document.fullscreenElement) {
                if (reportWorkspace.requestFullscreen) {
                    reportWorkspace.requestFullscreen();
                } else if (reportWorkspace.webkitRequestFullscreen) {
                    reportWorkspace.webkitRequestFullscreen();
                } else {
                    reportWorkspace.classList.toggle('is-fullscreen');
                }
            } else {
                if (document.exitFullscreen) {
                    document.exitFullscreen();
                }
            }
        });
    }

    if (refreshBtn) {
        refreshBtn.addEventListener('click', function () {
            const icon = this.querySelector('svg') || this;
            icon.classList.add('rotating');
            
            setTimeout(() => {
                icon.classList.remove('rotating');
            }, 650);
        });
    }
}

// Global retry action for error simulation
window.retryReportLoading = function (btn) {
    if (!btn) return;
    const originalText = btn.innerHTML;
    btn.innerHTML = `
        <svg class="rotating" width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round">
            <path d="M21.5 2v6h-6M21.34 15.57a10 10 0 1 1-.57-8.38l5.67-5.19"></path>
        </svg>
        <span>Tentative de reconnexion...</span>
    `;
    btn.disabled = true;

    setTimeout(() => {
        btn.innerHTML = originalText;
        btn.disabled = false;
        alert("La tentative de connexion à la source de données a échoué (serveur distant indisponible). Veuillez contacter l'administrateur système.");
    }, 1200);
};

/* ==========================================================================
   4. Chart Micro-Interactions & Tooltips
   ========================================================================== */
function initChartInteractions() {
    const tooltip = document.createElement('div');
    tooltip.className = 'chart-tooltip';
    document.body.appendChild(tooltip);

    document.querySelectorAll('.v-bar, .h-bar').forEach(bar => {
        bar.addEventListener('mouseenter', function () {
            const val = this.getAttribute('data-val') || 'Donnée';
            const label = this.getAttribute('data-label') || '';
            tooltip.innerHTML = `<strong>${label}</strong>: ${val}`;
            tooltip.style.display = 'block';
        });

        bar.addEventListener('mousemove', function (e) {
            tooltip.style.left = (e.pageX + 12) + 'px';
            tooltip.style.top = (e.pageY - 28) + 'px';
        });

        bar.addEventListener('mouseleave', function () {
            tooltip.style.display = 'none';
        });
    });
}

/* ==========================================================================
   5. Administration Features (Modals & Filtering)
   ========================================================================== */
let adminCurrentPage = 1;
let adminPageSize = 10;

function filterAndPaginateAdminTable() {
    const adminSearch = document.getElementById('adminTableSearch');
    const adminTypeFilter = document.getElementById('adminTypeFilter');
    const adminClearBtn = document.getElementById('adminSearchClear');

    const query = adminSearch ? adminSearch.value.toLowerCase().trim() : '';
    const selectedType = adminTypeFilter ? adminTypeFilter.value : '';

    if (adminClearBtn) {
        adminClearBtn.style.display = query.length > 0 ? 'inline-flex' : 'none';
    }

    const allRows = Array.from(document.querySelectorAll('.admin-table tbody tr'));
    if (allRows.length === 0) return;

    const matchingRows = allRows.filter(row => {
        const title = (row.getAttribute('data-title') || '').toLowerCase();
        const level = row.getAttribute('data-level') || '';

        const matchesText = !query || title.includes(query);
        const matchesType = !selectedType || level === selectedType;

        return matchesText && matchesType;
    });

    const totalMatching = matchingRows.length;
    const effectivePageSize = adminPageSize >= 1000 ? (totalMatching || 1) : adminPageSize;
    const totalPages = Math.ceil(totalMatching / effectivePageSize) || 1;

    if (adminCurrentPage > totalPages) adminCurrentPage = totalPages;
    if (adminCurrentPage < 1) adminCurrentPage = 1;

    const startIndex = (adminCurrentPage - 1) * effectivePageSize;
    const endIndex = Math.min(startIndex + effectivePageSize, totalMatching);

    allRows.forEach(row => row.style.display = 'none');
    matchingRows.slice(startIndex, endIndex).forEach(row => row.style.display = '');

    const startSpan = document.getElementById('paginationStart');
    const endSpan = document.getElementById('paginationEnd');
    const totalSpan = document.getElementById('paginationTotal');
    const navBtns = document.getElementById('paginationNav');

    if (startSpan) startSpan.textContent = totalMatching === 0 ? '0' : (startIndex + 1).toString();
    if (endSpan) endSpan.textContent = endIndex.toString();
    if (totalSpan) totalSpan.textContent = totalMatching.toString();

    if (navBtns) {
        let buttonsHtml = '';
        buttonsHtml += `<button type="button" class="page-nav-btn ${adminCurrentPage <= 1 ? 'disabled' : ''}" onclick="goToAdminPage(${adminCurrentPage - 1})" ${adminCurrentPage <= 1 ? 'disabled' : ''}>&laquo; Précédent</button>`;

        for (let p = 1; p <= totalPages; p++) {
            if (p === 1 || p === totalPages || (p >= adminCurrentPage - 2 && p <= adminCurrentPage + 2)) {
                buttonsHtml += `<button type="button" class="page-num-btn ${p === adminCurrentPage ? 'active' : ''}" onclick="goToAdminPage(${p})">${p}</button>`;
            } else if (p === adminCurrentPage - 3 || p === adminCurrentPage + 3) {
                buttonsHtml += `<span class="page-ellipsis">&hellip;</span>`;
            }
        }

        buttonsHtml += `<button type="button" class="page-nav-btn ${adminCurrentPage >= totalPages ? 'disabled' : ''}" onclick="goToAdminPage(${adminCurrentPage + 1})" ${adminCurrentPage >= totalPages ? 'disabled' : ''}>Suivant &raquo;</button>`;

        navBtns.innerHTML = buttonsHtml;
    }
}

window.goToAdminPage = function(page) {
    adminCurrentPage = page;
    filterAndPaginateAdminTable();
};

window.changePageSize = function(size) {
    adminPageSize = parseInt(size, 10) || 10;
    adminCurrentPage = 1;
    filterAndPaginateAdminTable();
};

function initAdminFeatures() {
    const adminSearch = document.getElementById('adminTableSearch');
    const adminTypeFilter = document.getElementById('adminTypeFilter');
    const adminClearBtn = document.getElementById('adminSearchClear');
    const pageSizeSelect = document.getElementById('pageSizeSelect');

    if (adminSearch) {
        adminSearch.addEventListener('input', function() {
            adminCurrentPage = 1;
            filterAndPaginateAdminTable();
        });
    }
    if (adminTypeFilter) {
        adminTypeFilter.addEventListener('change', function() {
            adminCurrentPage = 1;
            filterAndPaginateAdminTable();
        });
    }
    if (adminClearBtn) {
        adminClearBtn.addEventListener('click', function() {
            if (adminSearch) adminSearch.value = '';
            adminCurrentPage = 1;
            filterAndPaginateAdminTable();
            if (adminSearch) adminSearch.focus();
        });
    }
    if (pageSizeSelect) {
        pageSizeSelect.addEventListener('change', function() {
            window.changePageSize(this.value);
        });
    }

    filterAndPaginateAdminTable();
}

// Global modal helper functions for Admin
window.openAdminModal = function (modalId) {
    const modal = document.getElementById(modalId);
    if (modal) {
        modal.classList.add('open');
    }
};

window.closeAdminModal = function (modalId) {
    const modal = document.getElementById(modalId);
    if (modal) {
        modal.classList.remove('open');
    }
};

window.openAddChildModal = function (parentId, parentLevel) {
    const nextLevel = parentLevel == '1' ? '2' : '3';
    document.getElementById('addItemType').value = nextLevel;
    document.getElementById('addItemTitle').value = '';
    document.getElementById('addItemCode').value = '';
    document.getElementById('addItemDescription').value = '';
    document.getElementById('addItemOrder').value = '1';

    if (window.toggleAddParentSelector) {
        window.toggleAddParentSelector();
    }
    document.getElementById('addItemParentId').value = parentId || '';

    window.openAdminModal('modalAddItem');
};

/* ==========================================================================
   6. Clickable Breadcrumb Navigation Handler
   ========================================================================== */
window.handleBreadcrumbClick = function (e, nodeId) {
    if (e) e.preventDefault();
    if (!nodeId) return;

    const targetElement = document.querySelector(`[data-id="${nodeId}"]`);
    if (targetElement) {
        // Expand ancestor level 1 and level 2 items
        let parentL2 = targetElement.closest('.tree-level-2-item');
        if (parentL2) parentL2.classList.remove('collapsed');

        let parentL1 = targetElement.closest('.tree-level-1-item');
        if (parentL1) parentL1.classList.remove('collapsed');

        if (targetElement.classList.contains('collapsed')) {
            targetElement.classList.remove('collapsed');
        }

        // Find first available report link in this subtree
        const firstReport = targetElement.querySelector('.tree-report-link');
        if (firstReport) {
            const reportUrl = firstReport.getAttribute('href');
            if (reportUrl) {
                window.location.href = reportUrl;
                return;
            }
        }

        targetElement.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
    }
};

window.toggleSidebarMenuFromBreadcrumb = window.handleBreadcrumbClick;
