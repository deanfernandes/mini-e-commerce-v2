import { useNavigate, useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import { Product } from "../types/Product";
import { useAuth } from "../hooks/useAuth";

function ProductDetails() {
  const { id } = useParams<{ id: string }>();
  const [product, setProduct] = useState<Product | null>(null);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();
  const { token } = useAuth();

  useEffect(() => {
    const fetchProduct = async () => {
      try {
        const res = await fetch(`/api/products/${id}`, {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });
        const data = await res.json();
        setProduct(data);
      } catch (err) {
        console.error(`Failed to fetch product. ${(err as Error).message}`);
      } finally {
        setLoading(false);
      }
    };

    fetchProduct();
  }, [id]);

  const handleAddToCart = () => {
    // TODO:
    alert(`Added "${product?.name}" to cart!`);
  };

  if (loading) return <div className="p-4">Loading product...</div>;
  if (!product)
    return <div className="p-4 text-red-500">Product not found.</div>;

  return (
    <div className="p-4">
      <button
        onClick={() => navigate(-1)}
        className="text-primary hover:text-primary-light font-medium hover:underline cursor-pointer mb-4"
      >
        &larr; Back
      </button>

      <h2 className="text-2xl font-bold mb-2">{product.name}</h2>
      <p className="italic">{product.description}</p>
      <p className="text-lg font-semibold mt-2">${product.price.toFixed(2)}</p>

      <button
        onClick={handleAddToCart}
        className="mt-6 px-6 py-3 bg-primary text-gray-200 rounded hover:bg-primary-light transition cursor-pointer"
      >
        Add to Cart
      </button>
    </div>
  );
}

export default ProductDetails;
