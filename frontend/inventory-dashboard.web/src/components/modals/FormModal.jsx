export default function FormModal({
  show,
  onClose,
  title,
  size = "lg", // "sm" | "lg" | "xl"
  children,
}) {
  if (!show) return null;

  return (
    <>
      {/* Backdrop */}
      <div className="modal-backdrop fade show" onClick={onClose} />

      {/* Modal */}
      <div className="modal fade show d-block" tabIndex="-1" role="dialog">
        <div className={`modal-dialog modal-${size} modal-dialog-centered`}>
          <div className="modal-content">
            <div className="modal-header border-bottom-0 pb-0">
              <button type="button" className="btn-close" onClick={onClose} />
            </div>
            <div className="modal-body">{children}</div>
          </div>
        </div>
      </div>
    </>
  );
}
