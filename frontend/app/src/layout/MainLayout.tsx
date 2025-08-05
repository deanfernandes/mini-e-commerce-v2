import { Outlet } from "react-router-dom";
import ProductsNavBar from "../components/ProductsNavBar";

interface MainLayoutProps {
  token: string | null;
}

function MainLayout({ token }: MainLayoutProps) {
  return (
    <main className="flex-grow flex flex-col">
      {token && <ProductsNavBar />}
      <Outlet />
    </main>
  );
}

export default MainLayout;
