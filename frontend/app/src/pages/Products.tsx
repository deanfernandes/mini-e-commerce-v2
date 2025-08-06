import React, { useEffect, useState } from "react";
import { Product } from "../types/Product";
import ProductCard from "../components/ProductCard";
import { useAuth } from "../hooks/useAuth";

const Products: React.FC = () => {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const { token } = useAuth();
  const [currentPage, setCurrentPage] = useState<number>(1);
  const itemsPerPage = 5;

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

  const totalPages = Math.ceil(products.length / itemsPerPage);
  const startIndex = (currentPage - 1) * itemsPerPage;
  const currentProducts = products.slice(startIndex, startIndex + itemsPerPage);

  const goToPage = (page: number) => {
    if (page < 1 || page > totalPages) return;
    setCurrentPage(page);
  };

  if (loading) return <div className="p-4">Loading products...</div>;
  if (error) return <div className="p-4 text-red-500">{error}</div>;

  return (
    <div className="flex flex-col flex-grow p-4">
      <h2 className="text-2xl font-semibold mb-4">Products:</h2>

      {currentProducts.length === 0 ? (
        <div>No products found.</div>
      ) : (
        <>
          <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-4">
            {currentProducts.map((product) => (
              <ProductCard key={product.id} product={product} />
            ))}
          </div>

          <div className="mt-auto flex justify-center space-x-2 pt-6">
            <button
              onClick={() => goToPage(currentPage - 1)}
              disabled={currentPage === 1}
              className="px-3 py-1 border rounded cursor-pointer disabled:opacity-50 disabled:cursor-default"
            >
              Previous
            </button>

            {[...Array(totalPages)].map((_, idx) => {
              const page = idx + 1;
              return (
                <button
                  key={page}
                  onClick={() => goToPage(page)}
                  className={`px-3 py-1 border rounded ${
                    page === currentPage
                      ? "bg-primary-light text-gray-200"
                      : "cursor-pointer"
                  }`}
                >
                  {page}
                </button>
              );
            })}

            <button
              onClick={() => goToPage(currentPage + 1)}
              disabled={currentPage === totalPages}
              className="px-3 py-1 border rounded cursor-pointer disabled:opacity-50 disabled:cursor-default"
            >
              Next
            </button>
          </div>
        </>
      )}
    </div>
  );
};

export default Products;
