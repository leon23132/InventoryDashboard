import { NavLink } from "react-router-dom";

export default function Navbar() {
  const linkClass = ({ isActive }) =>
    "nav-link" + (isActive ? " active fw-semibold" : "");

  return (
    <nav className="navbar navbar-expand-lg navbar-dark bg-dark">
      <div className="container-fluid">
        <NavLink className="navbar-brand" to="/">
          Inventory Dashboard
        </NavLink>

        <button
          className="navbar-toggler"
          type="button"
          data-bs-toggle="collapse"
          data-bs-target="#navMenu"
          aria-controls="navMenu"
          aria-expanded="false"
          aria-label="Toggle navigation"
        >
          <span className="navbar-toggler-icon"></span>
        </button>

        <div className="collapse navbar-collapse" id="navMenu">
          <ul className="navbar-nav me-auto mb-2 mb-lg-0">
            <li className="nav-item">
              <NavLink to="/" className={linkClass} end>
                Home
              </NavLink>
            </li>

            <li className="nav-item">
              <NavLink to="/products" className={linkClass}>
                Products
              </NavLink>
            </li>

            <li className="nav-item">
              <NavLink to="/suppliers" className={linkClass}>
                Suppliers
              </NavLink>
            </li>

            <li className="nav-item">
              <NavLink to="/categories" className={linkClass}>
                Categories
              </NavLink>
            </li>

            <li className="nav-item">
              <NavLink to="/projects" className={linkClass}>
                Projects
              </NavLink>
            </li>
          </ul>
        </div>
      </div>
    </nav>
  );
}
