export function toggleSidebar() {
  const sidebar = document.querySelector('.side-bar');
  if (sidebar.style.display === 'flex')
    sidebar.style.display = 'none';
  else 
    sidebar.style.display = 'flex';
}

export function setSideBar(set) {
  const sidebar = document.querySelector('.side-bar');
  if (!set)
    sidebar.style.display = 'none';
  else 
    sidebar.style.display = 'flex';
}