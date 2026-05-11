import { useEffect } from "react";
import { useNavigate, useParams } from "react-router-dom";
import useSuppliers from "../../hooks/useSuppliers";
import { getWebsiteName } from "../../utils/url";
import PageTitle from "../../components/layout/PageTitle";
import HeaderActions from "../../components/layout/HeaderActions";
import BackButton from "../../components/Buttons/BackButton";
import EditButton from "../../components/Buttons/EditButton";
import PageLayout from "../../components/layout/PageLayout";
import ContentCard from "../../components/layout/ContentCard";
import DetailSection from "../../components/table/DetailSection";
import DetailField from "../../components/table/DetailField";

export default function SupplierDetailPage({ supplierId, isModal, onDone }) {
  const { getSupplierById, suppliers, loadingSuppliers, suppliersError } =
    useSuppliers();
  const params = useParams();
  const navigate = useNavigate();

  const effectiveId = supplierId ?? params.Id;

  useEffect(() => {
    getSupplierById(effectiveId);
  }, [effectiveId, getSupplierById]);

  return (
    <PageLayout>
      <PageTitle title="Detail Supplier">
        <HeaderActions>
          <BackButton
            onClick={() => (isModal ? onDone?.() : navigate("/suppliers"))}
          />
          <EditButton
            onClick={() => navigate(`/suppliers/${effectiveId}/edit`)}
            disabled={!loadingSuppliers && !!suppliersError}
          />
        </HeaderActions>
      </PageTitle>
      <ContentCard>
        {loadingSuppliers && <div>Loading supplier details...</div>}
        {suppliersError && (
          <div className="alert alert-danger mb-0">Error: {suppliersError}</div>
        )}

        {!loadingSuppliers && !suppliersError && (
          <>
            <DetailSection showDivider={true}>
              <DetailField label="Company Name">
                {suppliers?.companyName}
              </DetailField>
              <DetailField label="Email">{suppliers?.email}</DetailField>
              <DetailField label="Phone Number">
                {suppliers?.phoneNumber}
              </DetailField>
              <DetailField label="Website">
                <a
                  href={suppliers?.website}
                  target="_blank"
                  rel="noopener noreferrer"
                >
                  {getWebsiteName(suppliers?.website)}
                </a>
              </DetailField>
              <DetailField label="Contact Person">
                {suppliers?.contactPerson}
              </DetailField>
            </DetailSection>
            {/* Billing Address is mandatory, always show it */}
            <DetailSection title="Billing Address" showDivider={true}>
              <DetailField label="Street">
                {suppliers?.billingAddress?.streetAddress}
              </DetailField>
              <DetailField label="City">
                {suppliers?.billingAddress?.city}
              </DetailField>
              <DetailField label="Postal Code">
                {suppliers?.billingAddress?.postalCode}
              </DetailField>
              <DetailField label="Country">
                {suppliers?.billingAddress?.country}
              </DetailField>
            </DetailSection>
            {/* Shipping Address is optional, only show if it exists */}
            {suppliers?.shippingAddress && (
              <DetailSection title="Shipping Address" showDivider={false}>
                <DetailField label="Street">
                  {suppliers?.shippingAddress?.streetAddress}
                </DetailField>
                <DetailField label="City">
                  {suppliers?.shippingAddress?.city}
                </DetailField>
                <DetailField label="Postal Code">
                  {suppliers?.shippingAddress?.postalCode}
                </DetailField>
                <DetailField label="Country">
                  {suppliers?.shippingAddress?.country}
                </DetailField>
              </DetailSection>
            )}
          </>
        )}
      </ContentCard>
    </PageLayout>
  );
}
