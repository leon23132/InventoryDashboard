import FormModal from "./FormModal";

export default function ConfirmDeleteModal({
  show,
  title = "Confirm delete",
  message = "Are you sure you want to delete this item?",
  confirmText = "Delete",
  cancelText = "Cancel",
  onCancel,
  onConfirm,
  loading = false,
  danger = true,
  error = "",
}) {
  return (
    <FormModal show={show} onClose={onCancel} title={title} size="sm">
      <p className="mb-3">{message}</p>
      {error && <p className="text-danger small mb-3">{error}</p>}

      <div className="d-flex justify-content-end gap-2">
        <button
          type="button"
          className="btn btn-outline-secondary btn-sm"
          onClick={onCancel}
          disabled={loading}
        >
          {cancelText}
        </button>

        <button
          type="button"
          className={`btn btn-sm ${danger ? "btn-danger" : "btn-primary"}`}
          onClick={onConfirm}
          disabled={loading}
        >
          {loading ? "Deleting..." : confirmText}
        </button>
      </div>
    </FormModal>
  );
}
