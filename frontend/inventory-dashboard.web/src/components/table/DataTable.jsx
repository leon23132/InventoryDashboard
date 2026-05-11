import { useNavigate } from "react-router-dom";
import { normalizeUrl, getWebsiteName } from "../../utils/url";
export default function DataTable({
  items = [],
  rowKey = "id",
  labels = {},
  actions = {},
  onDelete = null,
  onEdit = null,
  onDetail = null,
  // For future pagination support
  page = 1,
  pageSize = 10,
  loading = false,
  onPageChange = null,
  showPagination = false,
}) {
  const navigate = useNavigate();

  const keyFn = typeof rowKey === "function" ? rowKey : (row) => row?.[rowKey];

  const fields = Object.keys(labels);

  const resolveUrl = (template, item) =>
    template ? template.replace(":id", item[rowKey]) : "";

  const renderValue = (value, field) => {
    if (value == null) return "";
    if (typeof value === "object") return value;
    if (field === "website") {
      const websiteUrl = getWebsiteName(value);
      if (!value) return "";
      return (
        <a href={websiteUrl} target="_blank" rel="noopener noreferrer">
          {websiteUrl}
        </a>
      );
    }
    return String(value);
  };

  const showActions =
    actions.edit || actions.details || onDelete || onEdit || onDetail;

  const canGoPrevious = page > 1 && !loading;
  const canGoNext = !loading && items.length === pageSize; // If we have less items than pageSize, we are likely on the last page

  return (
    <>
      {!items.length ? (
        <div className="text-muted">No data available.</div>
      ) : (
        <div className="table-responsive">
          <table className="table table-sm align-middle mb-0 table-spaced">
            <thead className="table-light">
              <tr>
                {fields.map((f) => (
                  <th
                    key={f}
                    className={
                      typeof labels[f] === "object" ? labels[f].className : ""
                    }
                  >
                    {typeof labels[f] === "object" ? labels[f].text : labels[f]}
                  </th>
                ))}
                {showActions && <th className="text-center"></th>}
              </tr>
            </thead>

            <tbody>
              {items.map((item) => (
                <tr key={keyFn(item)}>
                  {fields.map((f) => (
                    <td key={f}>{renderValue(item[f], f)}</td>
                  ))}

                  {showActions && (
                    <td className="text-end">
                      <div className="d-flex justify-content-end gap-2">
                        {(actions.edit || onEdit) && (
                          <button
                            className="btn btn-sm btn-outline-secondary"
                            onClick={() => {
                              if (onEdit) onEdit(item);
                              else if (actions.edit)
                                navigate(resolveUrl(actions.edit, item)); // <-- Route Edit
                            }}
                          >
                            Edit
                          </button>
                        )}

                        {onDelete && (
                          <button
                            className="btn btn-sm btn-outline-danger"
                            onClick={() => onDelete(item)}
                          >
                            Delete
                          </button>
                        )}

                        {actions.details && (
                          <button
                            className="btn btn-sm btn-outline-info"
                            onClick={() => {
                              if (onDetail) onDetail(item);
                              else navigate(resolveUrl(actions.details, item));
                            }}
                          >
                            Details
                          </button>
                        )}
                      </div>
                    </td>
                  )}
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
      {showPagination && onPageChange && (
        <div className="d-flex justify-content-between align-items-center mt-3">
          <button
            type="button"
            className="btn btn-outline-secondary btn-sm"
            disabled={!canGoPrevious}
            onClick={() => onPageChange(page - 1)}
          >
            Previous
          </button>

          <span className="small text-muted">Page {page}</span>

          <button
            type="button"
            className="btn btn-outline-secondary btn-sm"
            disabled={!canGoNext}
            onClick={() => onPageChange(page + 1)}
          >
            Next
          </button>
        </div>
      )}
    </>
  );
}
