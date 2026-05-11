import { Routes, Route } from "react-router-dom";
import Sidebar from "./components/Sidebar";

import HomePage from "./pages/HomePage";
import ProductsPage from "./pages/products/ProductsPage";
import ProductCreatePage from "./pages/products/ProductCreatePage";
import ProductDetailPage from "./pages/products/ProductDetailPage";
import SuppliersPage from "./pages/suppliers/SuppliersPage";
import CategoriesPage from "./pages/categories/CategoriesPage";
import ProjectsPage from "./pages/projects/ProjectsPage";
import SupplierCreatePage from "./pages/suppliers/SupplierCreatePage";
import SupplierDetailPage from "./pages/suppliers/SupplierDetailPage";
import CategoryCreatePage from "./pages/categories/CategoryCreatePage";
import CategoryDetailPage from "./pages/categories/CategoryDetailPage";
import ProjectCreatePage from "./pages/projects/ProjectCreatePage";
import ProjectDetailPage from "./pages/projects/ProjectDetailPage";

export default function App() {
  return (
    <div className="d-flex">
      <Sidebar />

      <main className="flex-grow-1 p-4 min-w-0 app-main">
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/products" element={<ProductsPage />} />
          <Route path="/suppliers" element={<SuppliersPage />} />
          <Route path="/categories" element={<CategoriesPage />} />
          <Route path="/projects" element={<ProjectsPage />} />
          <Route path="/projects/new" element={<ProjectCreatePage />} />
          <Route path="/projects/:Id" element={<ProjectDetailPage />} />
          <Route path="/projects/:Id/edit" element={<ProjectCreatePage />} />
          {/* Added routes for product creation and detail pages */}
          <Route path="/products/new" element={<ProductCreatePage />} />
          <Route path="/products/:Id/edit" element={<ProductCreatePage />} />
          <Route path="/products/:Id" element={<ProductDetailPage />} />
          {/* Added routes for supplier creation and detail pages */}
          <Route path="/suppliers/new" element={<SupplierCreatePage />} />
          <Route path="/suppliers/:Id/edit" element={<SupplierCreatePage />} />
          <Route path="/suppliers/:Id" element={<SupplierDetailPage />} />
          {/* Added routes for category creation and detail pages */}
          <Route path="/categories/new" element={<CategoryCreatePage />} />
          <Route path="/categories/:Id/edit" element={<CategoryCreatePage />} />
          <Route path="/categories/:Id" element={<CategoryDetailPage />} />
        </Routes>
      </main>
    </div>
  );
}
