import { NavLink } from "react-router-dom";

function ProductsNavBar() {
  return (
    <nav className="bg-primary-dark text-gray-200 flex space-x-6 py-2">
      <NavLink
        to="/products"
        className={({ isActive }) =>
          isActive
            ? "ml-2 underline pointer-events-none cursor-default"
            : "ml-2 hover:underline"
        }
      >
        Products
      </NavLink>
      <NavLink
        to="/favorites"
        className={({ isActive }) =>
          isActive
            ? "ml-2 underline pointer-events-none cursor-default"
            : "ml-2 hover:underline"
        }
      >
        Favorites
      </NavLink>
    </nav>
  );
}

export default ProductsNavBar;
