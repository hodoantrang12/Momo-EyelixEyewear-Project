function showSection(sectionName) {
            // Hide all sections
            const sections = document.querySelectorAll('.section-content');
            sections.forEach(section => section.classList.remove('active'));
            
            // Remove active class from all sidebar items
            const sidebarItems = document.querySelectorAll('.sidebar-item');
            sidebarItems.forEach(item => item.classList.remove('active'));
            
            // Show selected section
            const targetSection = document.getElementById(sectionName + '-section');
            if (targetSection) {
                targetSection.classList.add('active');
            }
            
            // Add active class to clicked sidebar item
            event.target.closest('.sidebar-item').classList.add('active');
        }

        function viewOrder(orderNumber) {
            alert('Viewing order #' + orderNumber + '\n\nThis would redirect to the order details page.');
        }

        function logout() {
            if (confirm('Are you sure you want to log out?')) {
                alert('Logging out...\n\nThis would redirect to the login page.');
            }
        }