import { Link } from "react-router-dom";

function ProductsNavBar() {
  return (
    <nav className="bg-primary-dark text-gray-200 flex space-x-6 py-2">
      <Link to="/products" className="ml-2 hover:underline">
        Products
      </Link>
      <Link to="/favorites" className="hover:underline">
        Favorites
      </Link>
    </nav>
  );
}

export default ProductsNavBar;
