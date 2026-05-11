import useCategories from "../../hooks/useCategories";
import { useEffect } from "react";
import { useNavigate, useParams } from "react-router-dom";
import PageTitle from "../../components/layout/PageTitle";
import PageLayout from "../../components/layout/PageLayout";
import ContentCard from "../../components/layout/ContentCard";
import HeaderActions from "../../components/layout/HeaderActions";
import BackButton from "../../components/Buttons/BackButton";
import EditButton from "../../components/Buttons/EditButton";
import DetailSection from "../../components/table/DetailSection";
import DetailField from "../../components/table/DetailField";

export default function CategoryDetailPage({ categoryId, isModal, onDone }) {
  // You can use the Id param to determine which category to fetch
  const { getCategoryById, categories, loadingCategories, categoriesError } =
    useCategories();
  const params = useParams();
  const navigate = useNavigate();

  const effectiveId = categoryId ?? params.id;

  useEffect(() => {
    getCategoryById(effectiveId);
  }, [effectiveId, getCategoryById]);

  return (
    <PageLayout>
      <PageTitle title="Detail Category">
        <HeaderActions>
          <BackButton
            onClick={() => (isModal ? onDone?.() : navigate("/categories"))}
          />
          <EditButton
            onClick={() => navigate(`/categories/${effectiveId}/edit`)}
            disabled={!loadingCategories && !!categoriesError}
          />
        </HeaderActions>
      </PageTitle>

      <ContentCard>
        {loadingCategories && <div>Loading category details...</div>}
        {categoriesError && (
          <div className="alert alert-danger mb-0">
            Error: {categoriesError}
          </div>
        )}

        {!loadingCategories && !categoriesError && (
          <>
            <DetailSection showDivider={false}>
              <DetailField label="Category Name">
                {categories?.name}
              </DetailField>
            </DetailSection>
          </>
        )}
      </ContentCard>
    </PageLayout>
  );
}
