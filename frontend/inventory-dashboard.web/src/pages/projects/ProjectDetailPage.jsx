import React from "react";
import PageLayout from "../../components/layout/PageLayout";
import PageTitle from "../../components/layout/PageTitle";
import ContentCard from "../../components/layout/ContentCard";
import { useNavigate, useParams } from "react-router-dom";
import useProjects from "../../hooks/useProjects";
import { useState, useEffect } from "react";
import FormInput from "../../components/form/FormInput";
import BackButton from "../../components/Buttons/BackButton";
import EditButton from "../../components/Buttons/EditButton";
import HeaderActions from "../../components/layout/HeaderActions";
import DetailSection from "../../components/table/DetailSection";
import DetailField from "../../components/table/DetailField";
import { formatCHF } from "../../utils/format";
import DataTable from "../../components/table/DataTable";
import { getWebsiteName } from "../../utils/url";
import { normalizeUrl } from "../../utils/url";

function ProjectDetailPage({ projectId, isModal, onDone }) {
  const params = useParams();
  const navigate = useNavigate();

  const effectiveId = projectId ?? params.Id;
  const { getProjectById, projects, loadingProjects, projectsError } =
    useProjects();
  useEffect(() => {
    getProjectById(effectiveId);
  }, [effectiveId, getProjectById]);

  return (
    <PageLayout>
      <PageTitle title="Project Details">
        <HeaderActions>
          <BackButton
            onClick={() => {
              if (isModal) {
                onDone?.(); // Modal schließen / Callback
              } else {
                navigate("/projects"); // Zurück zur Liste
              }
            }}
          />
          <EditButton
            onClick={() => navigate(`/projects/${effectiveId}/edit`)}
            disabled={loadingProjects && !projectsError}
          />
        </HeaderActions>
      </PageTitle>
      <ContentCard>
        {loadingProjects && <div>Loading project details...</div>}
        {projectsError && (
          <div className="alert alert-danger mb-0">Error: {projectsError}</div>
        )}

        {!loadingProjects && !projectsError && (
          <>
            <DetailSection title="General Information" showDivider={false}>
              <DetailField label="Project Name">
                {projects?.projectName}
              </DetailField>
              <DetailField label="Description">
                {projects?.description || (
                  <span className="text-muted">No description provided.</span>
                )}
              </DetailField>

              {/* Products */}
              <DetailSection title="Products" showDivider={false}>
                {projects.products?.length ? (
                  <div className="row g-3">
                    {projects.products.map((p) => (
                      <div
                        key={p.productId}
                        className="col-12 col-md-6 col-lg-4"
                      >
                        <ContentCard
                          disableHeader={false}
                          cardHeader={
                            <div className="d-flex justify-content-between align-items-center">
                              <span className="fw-semibold">
                                {p.productTitle}
                              </span>
                              <span className="badge text-bg-secondary">
                                x{p.quantity}
                              </span>
                            </div>
                          }
                        >
                          <div className="d-flex justify-content-between mb-2">
                            <span className="text-muted small">Unit Price</span>
                            <span>{formatCHF(p.unitPrice ?? 0)}</span>
                          </div>

                          <div className="d-flex justify-content-between">
                            <span className="text-muted small">Total</span>
                            <span className="fw-semibold">
                              {formatCHF(p.totalPrice ?? 0)}
                            </span>
                          </div>
                        </ContentCard>
                      </div>
                    ))}
                  </div>
                ) : (
                  <span className="text-muted">
                    No products associated with this project.
                  </span>
                )}
              </DetailSection>
            </DetailSection>
          </>
        )}
      </ContentCard>
    </PageLayout>
  );
}

export default ProjectDetailPage;
