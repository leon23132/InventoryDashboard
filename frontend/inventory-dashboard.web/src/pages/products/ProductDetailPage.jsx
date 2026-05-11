import { useState, useEffect } from "react";
import useProducts from "../../hooks/useProducts";
import { useNavigate, useParams } from "react-router-dom";
import PageTitle from "../../components/layout/PageTitle";
import PageLayout from "../../components/layout/PageLayout";
import ContentCard from "../../components/layout/ContentCard";
import HeaderActions from "../../components/layout/HeaderActions";
import BackButton from "../../components/Buttons/BackButton";
import EditButton from "../../components/Buttons/EditButton";
import { formatCHF } from "../../utils/format";
import DetailSection from "../../components/table/DetailSection";
import DetailField from "../../components/table/DetailField";

export default function ProductDetailPage({ productId, isModal, onDone }) {
  // State to hold the product ID
  const { getProductById, products, loadingProducts, productsError } =
    useProducts();

  const params = useParams();
  const navigate = useNavigate();

  const effectiveId = productId ?? params.Id;

  useEffect(() => {
    // Fetch product details by ID (example ID used here

    getProductById(effectiveId);
  }, [effectiveId, getProductById]);

  return (
    <PageLayout>
      <PageTitle title="Detail Product">
        <HeaderActions>
          <BackButton
            onClick={() => (isModal ? onDone?.() : navigate("/products"))}
          />
          <EditButton
            onClick={() => navigate(`/products/${effectiveId}/edit`)}
            disabled={!loadingProducts && !!productsError}
          />
        </HeaderActions>
      </PageTitle>

      <ContentCard>
        {loadingProducts && <div>Loading product details...</div>}

        {productsError && (
          <div className="alert alert-danger mb-0">Error: {productsError}</div>
        )}

        {!loadingProducts && !productsError && (
          <>
            <DetailSection showDivider={false}>
              <DetailField label="Product Name">
                {products?.productTitle}
              </DetailField>
              <DetailField label="Description">
                {products?.productDescription}
              </DetailField>
              <DetailField label="Product Category">
                {products?.categoryName}
              </DetailField>
              <DetailField label="Supplier">
                {products?.supplierName}
              </DetailField>
              <DetailField label="Price">
                {formatCHF(products?.price)}
              </DetailField>
              <DetailField label="Stock">
                {products?.quantityInStock}
              </DetailField>
              <DetailField label="Minimum Stock">
                {products?.minimumStock}
              </DetailField>
            </DetailSection>
          </>
        )}
      </ContentCard>
    </PageLayout>
  );
}
