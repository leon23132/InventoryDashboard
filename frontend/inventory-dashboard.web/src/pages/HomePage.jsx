import ContentCard from "../components/layout/ContentCard";
import PageLayout from "../components/layout/PageLayout";
import PageTitle from "../components/layout/PageTitle";
import useDashboard from "../hooks/useDashboard";
import CategoriesIcon from "../assets/icons/categoriesIcon.png";
import ProductsIcon from "../assets/icons/productsIcon.png";
import SuppliersIcon from "../assets/icons/suppliersIcon.png";
import OutOfStockIcon from "../assets/icons/outofstock.png";

import { Bar, Doughnut } from "react-chartjs-2";

export default function HomePage() {
  const { overview, loadingOverview, overviewError } = useDashboard();

  const barOptions = {
    responsive: true,
    maintainAspectRatio: true,
    plugins: {
      legend: { display: false },
      tooltip: { enabled: true },
    },
    scales: {
      x: { grid: { display: false } },
      y: { beginAtZero: true, ticks: { precision: 0 } },
    },
  };

  const doughnutOptions = {
    responsive: true,
    maintainAspectRatio: true,
    cutout: "65%",
    plugins: {
      legend: { position: "bottom" },
    },
  };

  const categoryChartData = {
    labels: overview?.productsPerCategory?.map((c) => c.categoryName) ?? [],
    datasets: [
      {
        label: "Products",
        data: overview?.productsPerCategory?.map((c) => c.productCount) ?? [],
        backgroundColor: "rgba(54, 162, 235, 0.7)",
        borderRadius: 8,
        maxBarThickness: 48,
      },
    ],
  };

  const topProductsChartData = {
    labels: overview?.topProductsByStock?.map((p) => p.name) ?? [],
    datasets: [
      {
        label: "Stock",
        data: overview?.topProductsByStock?.map((p) => p.stockQuantity) ?? [],
        backgroundColor: "rgba(255, 159, 64, 0.75)", // orange
        borderRadius: 8,
        maxBarThickness: 48,
      },
    ],
  };

  const supplierChartData = {
    labels: overview?.productsPerSupplier?.map((s) => s.supplierName) ?? [],
    datasets: [
      {
        label: "Products",
        data: overview?.productsPerSupplier?.map((s) => s.productCount) ?? [],
        backgroundColor: [
          "#4e79a7",
          "#f28e2b",
          "#e15759",
          "#76b7b2",
          "#59a14f",
          "#edc948",
          "#b07aa1",
          "#ff9da7",
        ],
        borderWidth: 0,
      },
    ],
  };

  return (
    <>
      <PageLayout>
        <PageTitle title="Dashboard" />
        <ContentCard className="mb-3">
          {!loadingOverview && !overviewError && (
            <div className="row g-3">
              <div className="col-12 col-md-6 col-xl-3">
                <ContentCard>
                  <div className="d-flex flex-column align-items-center text-center">
                    {/* ICON */}
                    <img
                      src={ProductsIcon}
                      alt="Products"
                      style={{
                        width: "80px",
                        height: "80px",
                        objectFit: "contain",
                      }}
                      className="mb-2"
                    />

                    {/* ZAHL */}
                    <h3 className="fw-bold mb-0">{overview?.totalProducts}</h3>

                    {/* TEXT */}
                    <small className="text-muted">Total Products</small>
                  </div>
                </ContentCard>
              </div>

              <div className="col-12 col-md-6 col-xl-3">
                <ContentCard>
                  <div className="d-flex flex-column align-items-center text-center">
                    <img
                      src={CategoriesIcon}
                      alt="Categories"
                      style={{ width: "80px", height: "80px" }}
                      className="mb-2"
                    />

                    <h3 className="fw-bold mb-0">
                      {overview?.totalCategories}
                    </h3>
                    <small className="text-muted">Total Categories</small>
                  </div>
                </ContentCard>
              </div>

              <div className="col-12 col-md-6 col-xl-3">
                <ContentCard>
                  <div className="d-flex flex-column align-items-center text-center">
                    <img
                      src={SuppliersIcon}
                      alt="Suppliers"
                      style={{ width: "80px", height: "80px" }}
                      className="mb-2"
                    />

                    <h3 className="fw-bold mb-0">{overview?.totalSuppliers}</h3>
                    <small className="text-muted">Total Suppliers</small>
                  </div>
                </ContentCard>
              </div>

              <div className="col-12 col-md-6 col-xl-3">
                <ContentCard>
                  <div className="d-flex flex-column align-items-center text-center">
                    <img
                      src={OutOfStockIcon}
                      alt="Low Stock"
                      style={{ width: "80px", height: "80px" }}
                      className="mb-2"
                    />

                    <h3 className="fw-bold mb-0">{overview?.lowStockCount}</h3>
                    <small className="text-muted">Low Stock Products</small>
                  </div>
                </ContentCard>
              </div>
            </div>
          )}
        </ContentCard>
        <ContentCard>
          {!loadingOverview && !overviewError && (
            <div className="row g-4">
              {/* Produkte pro Kategorie */}
              <div className="col-12 col-lg-6">
                <ContentCard>
                  <h5>Produkte pro Kategorie</h5>
                  <Bar data={categoryChartData} options={barOptions} />
                </ContentCard>
              </div>

              {/* Top Produkte */}
              <div className="col-12 col-lg-6">
                <ContentCard>
                  <h5>Top Produkte nach Bestand</h5>
                  <Bar data={topProductsChartData} options={barOptions} />
                </ContentCard>
              </div>

              {/* Supplier Doughnut */}
              <div className="col-12 col-lg-6">
                <ContentCard>
                  <h5>Produkte pro Lieferant</h5>
                  <Doughnut
                    data={supplierChartData}
                    options={doughnutOptions}
                  />
                </ContentCard>
              </div>
            </div>
          )}
        </ContentCard>
      </PageLayout>
    </>
  );
}
