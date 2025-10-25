import React, { useEffect, useRef } from "react";
import { createPortal } from "react-dom";

interface ErrorProps {
   open: boolean;
   title?: string;
   message?: string;
   onClose?: () => void;
   onRetry?: () => void;
   closeLabel?: string;
   retryLabel?: string;
}

/**
 * Простой диалог ошибки. Рендерится в портал в document.body.
 * Использование:
 * <Error open={!!error} message={errorMessage} onClose={() => setError(null)} onRetry={retryFn} />
 */
const Error: React.FC<ErrorProps> = ({
   open,
   title = "Ошибка",
   message = "Произошла ошибка. Попробуйте ещё раз.",
   onClose,
   onRetry,
   closeLabel = "Закрыть",
   retryLabel = "Повторить",
}) => {
   const closeBtnRef = useRef<HTMLButtonElement | null>(null);
   const overlayRef = useRef<HTMLDivElement | null>(null);

   useEffect(() => {
      if (!open) return;
      // фокус на кнопку закрытия
      closeBtnRef.current?.focus();

      const onKey = (e: KeyboardEvent) => {
         if (e.key === "Escape") onClose?.();
      };
      document.addEventListener("keydown", onKey);
      return () => document.removeEventListener("keydown", onKey);
   }, [open, onClose]);

   if (!open) return null;
   if (typeof document === "undefined") return null;

   const overlayStyle: React.CSSProperties = {
      position: "fixed",
      inset: 0,
      background: "rgba(0,0,0,0.45)",
      display: "flex",
      alignItems: "center",
      justifyContent: "center",
      zIndex: 9999,
      padding: 16,
   };

   const dialogStyle: React.CSSProperties = {
      width: "100%",
      maxWidth: 540,
      background: "#fff",
      borderRadius: 12,
      boxShadow: "0 10px 30px rgba(2,6,23,0.2)",
      overflow: "hidden",
   };

   const headerStyle: React.CSSProperties = {
      padding: "16px 20px",
      borderBottom: "1px solid rgba(0,0,0,0.06)",
      fontWeight: 600,
      fontSize: 18,
   };

   const bodyStyle: React.CSSProperties = {
      padding: "16px 20px",
      color: "#0f1724",
      fontSize: 14,
      lineHeight: 1.4,
   };

   const footerStyle: React.CSSProperties = {
      padding: "12px 16px",
      display: "flex",
      gap: 8,
      justifyContent: onRetry ? "space-between" : "flex-end",
      borderTop: "1px solid rgba(0,0,0,0.04)",
      background: "#fafafa",
   };

   const btnStyle: React.CSSProperties = {
      padding: "8px 12px",
      borderRadius: 8,
      border: "1px solid rgba(0,0,0,0.08)",
      background: "#fff",
      cursor: "pointer",
   };

   const retryStyle: React.CSSProperties = {
      ...btnStyle,
      background: "#2563eb",
      color: "#fff",
      border: "none",
   };

   const onOverlayClick = (e: React.MouseEvent) => {
      if (e.target === overlayRef.current) onClose?.();
   };

   const dialog = (
      <div
         ref={overlayRef}
         style={overlayStyle}
         role="dialog"
         aria-modal="true"
         aria-labelledby="error-title"
         onMouseDown={onOverlayClick}
      >
         <div style={dialogStyle} onMouseDown={(e) => e.stopPropagation()}>
            <div style={headerStyle} id="error-title">
               {title}
            </div>
            <div style={bodyStyle}>
               <div>{message}</div>
            </div>
            <div style={footerStyle}>
               {onRetry ? (
                  <>
                     <div style={{ display: "flex", gap: 8 }}>
                        <button
                           type="button"
                           onClick={onClose}
                           style={btnStyle}
                           ref={closeBtnRef}
                        >
                           {closeLabel}
                        </button>
                        <button
                           type="button"
                           onClick={onRetry}
                           style={retryStyle}
                        >
                           {retryLabel}
                        </button>
                     </div>
                     <div style={{ color: "rgba(15,23,36,0.6)", fontSize: 12, alignSelf: "center" }}>
                        Если проблема повторяется — проверьте соединение или обратитесь в поддержку.
                     </div>
                  </>
               ) : (
                  <div style={{ marginLeft: "auto" }}>
                     <button
                        type="button"
                        onClick={onClose}
                        style={btnStyle}
                        ref={closeBtnRef}
                     >
                        {closeLabel}
                     </button>
                  </div>
               )}
            </div>
         </div>
      </div>
   );

   return createPortal(dialog, document.body);
};

export default Error;