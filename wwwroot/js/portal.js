/**
 * SOCADEL Reporting Portal - Master JavaScript
 * Handles navigation tree, enhanced search with highlighting & instant dropdown,
 * workspace actions, chart micro-interactions, auth/roles, and admin features.
 */

document.addEventListener('DOMContentLoaded', function () {
    initAuthSystem();
    initTreeNavigation();
    initSearchFilter();
    initActionButtons();
    initChartInteractions();
    initAdminFeatures();
});

/* ==========================================================================
   0. Text Normalization Helper (Case & Accent Insensitive)
   ========================================================================== */
function normalizeText(text) {
    if (!text) return '';
    return text.toString()
        .normalize("NFD")
        .replace(/[\u0300-\u036f]/g, "")
        .toLowerCase();
}

/* ==========================================================================
   1. Authentication & Role Switcher System (Client-side Frontend Role State)
   ========================================================================== */
function applyRoleUI(role) {
    document.body.setAttribute('data-user-role', role);

    const adminSidebarBox = document.querySelector('.sidebar-admin-box');
    const headerAdminBtn = document.querySelector('.header-admin-action-btn.go-to-admin');
    const userBadgeName = document.querySelector('.user-single-name');
    const userAvatar = document.querySelector('.user-avatar-circle span');

    if (role === 'admin') {
        if (adminSidebarBox) adminSidebarBox.style.display = 'block';
        if (headerAdminBtn) headerAdminBtn.style.display = 'inline-flex';
        if (userBadgeName) userBadgeName.textContent = 'Naomi Tsague';
    } else {
        if (adminSidebarBox) adminSidebarBox.style.display = 'none';
        if (headerAdminBtn) headerAdminBtn.style.display = 'none';
        if (userBadgeName) userBadgeName.textContent = 'Naomi Tsague';

        // Restrict access to /Admin page if in simple user role
        if (window.location.pathname.toLowerCase().startsWith('/admin')) {
            window.location.href = '/?access_restricted=1';
        }
    }
}

function initAuthSystem() {
    let currentRole = localStorage.getItem('socadel_user_role') || 'user';
    const isLoggedIn = localStorage.getItem('socadel_logged_in') === 'true';
    const loginModal = document.getElementById('modalAppLogin');

    if (!isLoggedIn) {
        if (loginModal) loginModal.style.display = 'flex';
    } else {
        if (loginModal) loginModal.style.display = 'none';
    }

    applyRoleUI(currentRole);

    // Profile badge displays name only
    const userBadge = document.querySelector('.header-user-badge');
    if (userBadge) {
        userBadge.style.cursor = 'default';
    }

    window.switchRole = function (newRole) {
        currentRole = newRole;
        localStorage.setItem('socadel_user_role', newRole);
        applyRoleUI(newRole);
        closeAuthModal();
        showToast(newRole === 'admin' ? "Connecté en tant qu'Administrateur" : "Connecté en tant que Simple Utilisateur");
    };
}

window.handleLoginSubmit = function (e) {
    e.preventDefault();
    const emailInput = document.getElementById('loginEmail');
    const passwordInput = document.getElementById('loginPassword');
    const alertBox = document.getElementById('loginAlert');
    if (!emailInput || !passwordInput) return;

    const email = emailInput.value.trim().toLowerCase();
    const password = passwordInput.value.trim();

    let role = 'user';
    if (email === 'admin@socadel.cm' || email.includes('admin')) {
        role = 'admin';
    } else {
        role = 'user';
    }

    localStorage.setItem('socadel_logged_in', 'true');
    localStorage.setItem('socadel_user_role', role);
    localStorage.setItem('socadel_user_email', email);

    const loginModal = document.getElementById('modalAppLogin');
    if (loginModal) loginModal.style.display = 'none';

    applyRoleUI(role);
    showToast(`Bienvenue ! Connecté en tant que ${role === 'admin' ? 'Administrateur' : 'Simple Utilisateur'}`);

    if (role === 'admin' && !window.location.pathname.toLowerCase().startsWith('/admin')) {
        window.location.href = '/Admin';
    } else if (role === 'user' && window.location.pathname.toLowerCase().startsWith('/admin')) {
        window.location.href = '/';
    }
};

window.fillLoginCredentials = function (email, password) {
    const emailInput = document.getElementById('loginEmail');
    const passwordInput = document.getElementById('loginPassword');
    if (emailInput) emailInput.value = email;
    if (passwordInput) passwordInput.value = password;
};

window.logoutApp = function () {
    localStorage.removeItem('socadel_logged_in');
    showToast("Vous êtes déconnecté.");
    const loginModal = document.getElementById('modalAppLogin');
    if (loginModal) loginModal.style.display = 'flex';
    if (window.location.pathname.toLowerCase().startsWith('/admin')) {
        window.location.href = '/';
    }
};

function openAuthModal() {
    let modal = document.getElementById('modalAuthRole');
    if (!modal) return;
    modal.classList.add('open');
}

function closeAuthModal() {
    let modal = document.getElementById('modalAuthRole');
    if (!modal) return;
    modal.classList.remove('open');
}

window.closeAuthModal = closeAuthModal;

/* ==========================================================================
   2. Tree Navigation (Multi-Level Recursive & Universal Toggles)
   ========================================================================== */
function initTreeNavigation() {
    // Universal Event Delegation for ALL tree carets & headers at ANY level of depth!
    document.addEventListener('click', function (e) {
        const header = e.target.closest('.tree-cat-header, .tree-sub-header, .tree-caret');
        if (header) {
            e.preventDefault();
            e.stopPropagation();
            const parentItem = header.closest('.tree-level-1-item, .tree-level-2-item, .tree-sub-item, .tree-node-item');
            if (parentItem) {
                parentItem.classList.toggle('collapsed');
            }
        }
    });

    // Active item ancestor expansion
    const activeReport = document.querySelector('.tree-report-link.active');
    if (activeReport) {
        let p = activeReport.parentElement;
        while (p && !p.classList.contains('sidebar-nav-container')) {
            if (p.classList.contains('collapsed')) {
                p.classList.remove('collapsed');
            }
            p = p.parentElement;
        }
    }

    // Report Links: Smooth iframe update
    document.querySelectorAll('.tree-report-link').forEach(link => {
        link.addEventListener('click', function (e) {
            const reportId = this.getAttribute('data-id');
            const pbiContainer = document.querySelector('.pbi-report-container');

            if (pbiContainer && reportId) {
                e.preventDefault();

                document.querySelectorAll('.tree-report-link').forEach(r => r.classList.remove('active'));
                this.classList.add('active');

                // Expand ancestors
                let p = this.parentElement;
                while (p && !p.classList.contains('sidebar-nav-container')) {
                    if (p.classList.contains('collapsed')) {
                        p.classList.remove('collapsed');
                    }
                    p = p.parentElement;
                }

                window.history.pushState({ reportId }, '', `/?report=${encodeURIComponent(reportId)}`);

                const reportTitle = this.querySelector('.report-link-text')?.textContent.trim() || 'Rapport décisionnel';
                const topTitle = document.querySelector('.pbi-report-title-top');
                if (topTitle) topTitle.textContent = reportTitle;

                const currentBreadcrumb = document.querySelector('.breadcrumb-current-page');
                if (currentBreadcrumb) currentBreadcrumb.textContent = reportTitle;

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
   3. Enhanced Search Filtering (Accent & Case Insensitive)
   ========================================================================== */
function initSearchFilter() {
    const sidebarInput = document.getElementById('sidebarSearchInput');
    const sidebarClear = document.getElementById('sidebarSearchClear');
    const sidebarBadge = document.getElementById('sidebarSearchBadge');

    const headerInput = document.getElementById('headerSearchInput');
    const headerClear = document.getElementById('headerSearchClear');
    const headerResults = document.getElementById('headerSearchResults');

    function performTreeSearch(query) {
        const term = normalizeText(query).trim();
        const reportItems = document.querySelectorAll('.tree-level-3-item, .tree-report-item');
        const subItems = document.querySelectorAll('.tree-level-2-item, .tree-sub-item');
        const catItems = document.querySelectorAll('.tree-level-1-item, .tree-cat-item');

        if (sidebarClear) sidebarClear.style.display = term ? 'flex' : 'none';
        if (headerClear) headerClear.style.display = term ? 'flex' : 'none';

        if (!term) {
            if (sidebarBadge) sidebarBadge.style.display = 'none';

            reportItems.forEach(item => {
                item.style.display = '';
                const link = item.querySelector('.report-link-text');
                if (link && link.getAttribute('data-original')) {
                    link.innerHTML = link.getAttribute('data-original');
                }
            });

            subItems.forEach(item => item.style.display = '');
            catItems.forEach(item => item.style.display = '');
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
            const normalizedDataTitle = normalizeText(item.getAttribute('data-title') || rawText);

            if (normalizedDataTitle.includes(term)) {
                matchCount++;
                item.style.display = '';

                // Expand parent elements
                let p = item.parentElement;
                while (p && !p.classList.contains('sidebar-nav-container')) {
                    if (p.classList.contains('collapsed')) {
                        p.classList.remove('collapsed');
                    }
                    p.style.display = '';
                    p = p.parentElement;
                }
            } else {
                item.style.display = 'none';
            }
        });

        // Hide empty parent subcategories & categories
        subItems.forEach(sub => {
            const hasVisibleChildren = sub.querySelectorAll('li:not([style*="display: none"])').length > 0;
            if (!hasVisibleChildren) {
                sub.style.display = 'none';
            }
        });

        catItems.forEach(cat => {
            const hasVisibleChildren = cat.querySelectorAll('li:not([style*="display: none"])').length > 0;
            if (!hasVisibleChildren) {
                cat.style.display = 'none';
            }
        });

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
        const term = normalizeText(query).trim();

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
            
            const subTitle = link.closest('.tree-level-2-item, .tree-sub-item')?.getAttribute('data-title') || '';
            const catTitle = link.closest('.tree-level-1-item, .tree-cat-item')?.getAttribute('data-title') || '';

            if (normalizeText(dataTitle).includes(term) || normalizeText(catTitle).includes(term) || normalizeText(subTitle).includes(term)) {
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

    function escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

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
   4. Action Buttons (Fullscreen with "Réduire" Button & Real Refresh)
   ========================================================================== */
function initActionButtons() {
    const fullscreenBtn = document.getElementById('btnFullscreen');
    const refreshBtn = document.getElementById('btnRefresh');
    const reportWorkspace = document.querySelector('.report-workspace');

    // Create floating "Réduire" button for Fullscreen mode
    let reduceBtn = document.getElementById('btnReduceFullscreen');
    if (!reduceBtn && reportWorkspace) {
        reduceBtn = document.createElement('button');
        reduceBtn.id = 'btnReduceFullscreen';
        reduceBtn.type = 'button';
        reduceBtn.className = 'btn-reduce-fullscreen';
        reduceBtn.title = 'Réduire le plein écran';
        reduceBtn.innerHTML = `
            <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round">
                <polyline points="4 14 10 14 10 20"></polyline>
                <polyline points="20 10 14 10 14 4"></polyline>
                <line x1="14" y1="10" x2="21" y2="3"></line>
                <line x1="3" y1="21" x2="10" y2="14"></line>
            </svg>
            <span>Réduire</span>
        `;
        reduceBtn.style.display = 'none';
        reportWorkspace.appendChild(reduceBtn);

        reduceBtn.addEventListener('click', function () {
            exitFullscreenMode();
        });
    }

    function toggleFullscreenMode() {
        if (!document.fullscreenElement && (!reportWorkspace || !reportWorkspace.classList.contains('is-fullscreen'))) {
            if (reportWorkspace && reportWorkspace.requestFullscreen) {
                reportWorkspace.requestFullscreen();
            } else if (reportWorkspace && reportWorkspace.webkitRequestFullscreen) {
                reportWorkspace.webkitRequestFullscreen();
            } else if (reportWorkspace) {
                reportWorkspace.classList.add('is-fullscreen');
            }
            if (reduceBtn) reduceBtn.style.display = 'inline-flex';
        } else {
            exitFullscreenMode();
        }
    }

    function exitFullscreenMode() {
        if (document.fullscreenElement) {
            if (document.exitFullscreen) {
                document.exitFullscreen();
            } else if (document.webkitExitFullscreen) {
                document.webkitExitFullscreen();
            }
        }
        if (reportWorkspace) reportWorkspace.classList.remove('is-fullscreen');
        if (reduceBtn) reduceBtn.style.display = 'none';
    }

    document.addEventListener('fullscreenchange', function () {
        if (document.fullscreenElement) {
            if (reduceBtn) reduceBtn.style.display = 'inline-flex';
        } else {
            if (reduceBtn) reduceBtn.style.display = 'none';
        }
    });

    if (fullscreenBtn) {
        fullscreenBtn.addEventListener('click', toggleFullscreenMode);
    }

    if (refreshBtn) {
        refreshBtn.addEventListener('click', function () {
            const icon = this.querySelector('svg') || this;
            icon.classList.add('rotating');

            // Actualiser réellement l'iframe du rapport si présente
            const iframe = document.querySelector('.pbi-iframe-wrapper iframe');
            if (iframe && iframe.src) {
                const currentSrc = iframe.src;
                iframe.style.opacity = '0.4';
                iframe.src = '';
                setTimeout(() => {
                    iframe.src = currentSrc;
                    iframe.style.opacity = '1';
                }, 200);
            }

            showToast("Rapport actualisé avec succès");

            setTimeout(() => {
                icon.classList.remove('rotating');
            }, 650);
        });
    }
}

function showToast(message) {
    let toast = document.getElementById('socadelToast');
    if (!toast) {
        toast = document.createElement('div');
        toast.id = 'socadelToast';
        toast.className = 'socadel-toast';
        document.body.appendChild(toast);
    }
    toast.textContent = message;
    toast.classList.add('show');
    setTimeout(() => {
        toast.classList.remove('show');
    }, 3000);
}

// Global retry action
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
        showToast("Échec de connexion au serveur de données.");
    }, 1200);
};

/* ==========================================================================
   5. Chart Micro-Interactions
   ========================================================================== */
function initChartInteractions() {
    let tooltip = document.querySelector('.chart-tooltip');
    if (!tooltip) {
        tooltip = document.createElement('div');
        tooltip.className = 'chart-tooltip';
        document.body.appendChild(tooltip);
    }

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
   6. Administration Features (Datatable Search Accent-Insensitive & Pagination)
   ========================================================================== */
let adminCurrentPage = 1;
let adminPageSize = 10;

function filterAndPaginateAdminTable() {
    const adminSearch = document.getElementById('adminTableSearch');
    const adminTypeFilter = document.getElementById('adminTypeFilter');
    const adminClearBtn = document.getElementById('adminSearchClear');

    const query = normalizeText(adminSearch ? adminSearch.value : '').trim();
    const selectedType = adminTypeFilter ? adminTypeFilter.value : '';

    if (adminClearBtn) {
        adminClearBtn.style.display = query.length > 0 ? 'inline-flex' : 'none';
    }

    const allRows = Array.from(document.querySelectorAll('.admin-table tbody tr'));
    if (allRows.length === 0) return;

    const matchingRows = allRows.filter(row => {
        const title = normalizeText(row.getAttribute('data-title') || '');
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
   7. Clickable Breadcrumb Handler
   ========================================================================== */
window.handleBreadcrumbClick = function (e, nodeId) {
    if (e) e.preventDefault();
    if (!nodeId) return;

    const targetElement = document.querySelector(`[data-id="${nodeId}"]`);
    if (targetElement) {
        let p = targetElement.parentElement;
        while (p && !p.classList.contains('sidebar-nav-container')) {
            if (p.classList.contains('collapsed')) {
                p.classList.remove('collapsed');
            }
            p = p.parentElement;
        }

        if (targetElement.classList.contains('collapsed')) {
            targetElement.classList.remove('collapsed');
        }

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
