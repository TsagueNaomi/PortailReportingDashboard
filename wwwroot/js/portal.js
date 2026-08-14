/**
 * SOCADEL Reporting Portal - Master JavaScript
 * Handles navigation tree interactions, search, fullscreen, refresh simulation, user menu & admin modals
 */

document.addEventListener('DOMContentLoaded', function () {
    initTreeNavigation();
    initSearchFilter();
    initActionButtons();
    initUserMenu();
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

    // Ensure active item and its parents are expanded on load
    const activeReport = document.querySelector('.tree-report-link.active');
    if (activeReport) {
        let parent = activeReport.parentElement;
        while (parent && !parent.classList.contains('sidebar-nav-container')) {
            if (parent.classList.contains('collapsed')) {
                parent.classList.remove('collapsed');
            }
            parent = parent.parentElement;
        }
    }
}

/* ==========================================================================
   2. Search Filtering (Sidebar & Header)
   ========================================================================== */
function initSearchFilter() {
    const sidebarSearchInput = document.getElementById('sidebarSearchInput');
    const headerSearchInput = document.getElementById('headerSearchInput');

    function filterTree(query) {
        query = (query || '').trim().toLowerCase();
        const reportLinks = document.querySelectorAll('.tree-report-link');
        const subItems = document.querySelectorAll('.tree-level-2-item');
        const catItems = document.querySelectorAll('.tree-level-1-item');

        if (!query) {
            // Reset visibility
            document.querySelectorAll('.tree-level-1-item, .tree-level-2-item, .tree-level-3-item').forEach(el => {
                el.style.display = '';
            });
            return;
        }

        // Search through reports and subcategories
        reportLinks.forEach(link => {
            const title = (link.getAttribute('data-title') || link.textContent).toLowerCase();
            const parentItem = link.closest('.tree-level-3-item');
            if (title.includes(query)) {
                parentItem.style.display = '';
                // Ensure parents are visible and expanded
                let p = parentItem.parentElement;
                while (p && !p.classList.contains('sidebar-nav-container')) {
                    if (p.classList.contains('collapsed')) {
                        p.classList.remove('collapsed');
                    }
                    p.style.display = '';
                    p = p.parentElement;
                }
            } else {
                parentItem.style.display = 'none';
            }
        });
    }

    if (sidebarSearchInput) {
        sidebarSearchInput.addEventListener('input', function () {
            filterTree(this.value);
        });
    }

    if (headerSearchInput) {
        headerSearchInput.addEventListener('input', function () {
            filterTree(this.value);
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
            
            // Simulate reload / refresh
            setTimeout(() => {
                icon.classList.remove('rotating');
            }, 650);
        });
    }
}

/* ==========================================================================
   4. User Menu Dropdown
   ========================================================================== */
function initUserMenu() {
    const userSection = document.getElementById('headerUserSection');
    const userDropdown = document.getElementById('userDropdownMenu');

    if (userSection && userDropdown) {
        userSection.addEventListener('click', function (e) {
            e.stopPropagation();
            userDropdown.classList.toggle('show');
        });

        document.addEventListener('click', function (e) {
            if (!userDropdown.contains(e.target) && !userSection.contains(e.target)) {
                userDropdown.classList.remove('show');
            }
        });
    }
}

/* ==========================================================================
   5. Chart Tooltips & Micro-Interactions
   ========================================================================== */
function initChartInteractions() {
    const tooltip = document.createElement('div');
    tooltip.className = 'chart-tooltip';
    document.body.appendChild(tooltip);

    // Bars hover
    document.querySelectorAll('.v-bar, .h-bar').forEach(bar => {
        bar.addEventListener('mouseenter', function (e) {
            const val = this.getAttribute('data-val') || 'Donnée';
            const label = this.getAttribute('data-label') || '';
            tooltip.innerHTML = `<strong>${label}</strong>: ${val}`;
            tooltip.style.display = 'block';
        });

        bar.addEventListener('mousemove', function (e) {
            tooltip.style.left = (e.pageX + 10) + 'px';
            tooltip.style.top = (e.pageY - 25) + 'px';
        });

        bar.addEventListener('mouseleave', function () {
            tooltip.style.display = 'none';
        });
    });
}

/* ==========================================================================
   6. Administration Features (Modals & Filtering)
   ========================================================================== */
function initAdminFeatures() {
    const adminSearch = document.getElementById('adminTableSearch');
    const adminTypeFilter = document.getElementById('adminTypeFilter');

    function filterAdminTable() {
        if (!adminSearch && !adminTypeFilter) return;
        const query = adminSearch ? adminSearch.value.toLowerCase().trim() : '';
        const selectedType = adminTypeFilter ? adminTypeFilter.value : '';

        const rows = document.querySelectorAll('.admin-table tbody tr');
        rows.forEach(row => {
            const title = (row.getAttribute('data-title') || '').toLowerCase();
            const level = row.getAttribute('data-level') || '';
            const matchesText = !query || title.includes(query);
            const matchesType = !selectedType || level === selectedType;

            if (matchesText && matchesType) {
                row.style.display = '';
            } else {
                row.style.display = 'none';
            }
        });
    }

    if (adminSearch) {
        adminSearch.addEventListener('input', filterAdminTable);
    }
    if (adminTypeFilter) {
        adminTypeFilter.addEventListener('change', filterAdminTable);
    }
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

window.openEditModal = function (id, title, parentId, type, order, engine, description) {
    document.getElementById('editItemId').value = id || '';
    document.getElementById('editItemTitle').value = title || '';
    document.getElementById('editItemParentId').value = parentId || '';
    document.getElementById('editItemType').value = type || '3';
    document.getElementById('editItemOrder').value = order || '1';
    document.getElementById('editItemEngine').value = engine || 'PowerBI';
    document.getElementById('editItemDescription').value = description || '';

    window.openAdminModal('modalEditItem');
};

window.openAddChildModal = function (parentId, parentLevel) {
    const nextType = parentLevel == '1' ? '2' : '3';
    document.getElementById('addItemParentId').value = parentId || '';
    document.getElementById('addItemType').value = nextType;
    document.getElementById('addItemTitle').value = '';
    document.getElementById('addItemDescription').value = '';
    
    window.openAdminModal('modalAddItem');
};
