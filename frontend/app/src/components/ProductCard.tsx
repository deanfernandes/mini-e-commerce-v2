import { Link } from "react-router-dom";
import { Product } from "../types/Product";

interface ProductCardProps {
  product: Product;
}

function ProductCard({ product }: ProductCardProps) {
  return (
    <div className="rounded-lg shadow-lg p-4">
      <h3 className="mt-2 text-lg font-semibold">{product.name}</h3>
      <Link
        to={`${product.id}`}
        className="inline-block mt-3 text-primary hover:text-primary-light font-medium hover:underline"
      >
        View details
      </Link>
    </div>
  );
}

export default ProductCard;
