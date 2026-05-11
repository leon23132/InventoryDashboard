import { NavLink } from "react-router-dom";

export default function Sidebar() {
  const linkClass = ({ isActive }) =>
    `
      d-flex align-items-center
      px-3 py-2 rounded-3
      text-decoration-none
      transition
      ${isActive
        ? "bg-primary bg-opacity-10 text-primary fw-semibold"
        : "text-muted hover-link"}
    `;

  return (
    <aside className="sidebar">
      <div className="sidebar-header">
        <div className="sidebar-title">Inventory</div>
        <div className="sidebar-subtitle">Dashboard</div>
      </div>

      <nav className="nav flex-column gap-1 px-2">
        <NavLink to="/" end className={linkClass}>
          Home
        </NavLink>
        <NavLink to="/products" className={linkClass}>
          Products
        </NavLink>
        <NavLink to="/suppliers" className={linkClass}>
          Suppliers
        </NavLink>
        <NavLink to="/categories" className={linkClass}>
          Categories
        </NavLink>
        <NavLink to="/projects" className={linkClass}>
          Projects
        </NavLink>
      </nav>
    </aside>
  );
}
