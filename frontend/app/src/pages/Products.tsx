import React, { useEffect, useState } from "react";
import { Product } from "../types/Product";
import ProductCard from "../components/ProductCard";
import { useAuth } from "../hooks/useAuth";

const Products: React.FC = () => {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const { token } = useAuth();

  useEffect(() => {
    const fetchProducts = async () => {
      try {
        const res = await fetch("/api/products/", {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });

        const data = await res.json();
        const transformed = data.map((item: any) => ({
          id: item.productId,
          name: item.name,
          description: item.description,
          price: item.price,
        }));

        setProducts(transformed);
      } catch (err) {
        console.error(`Failed to fetch products. ${(err as Error).message}`);
        setError("Failed to fetch products.");
      } finally {
        setLoading(false);
      }
    };

    fetchProducts();
  }, []);

  if (loading) return <div className="p-4">Loading products...</div>;
  if (error) return <div className="p-4 text-red-500">{error}</div>;

  return (
    <div className="p-4">
      <h2 className="text-2xl font-semibold mb-4">Products:</h2>

      {loading ? (
        <div>Loading products...</div>
      ) : error ? (
        <div className="text-red-500">{error}</div>
      ) : (
        <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-4">
          {products.map((product) => (
            <ProductCard key={product.id} product={product} />
          ))}
        </div>
      )}
    </div>
  );
};

export default Products;
