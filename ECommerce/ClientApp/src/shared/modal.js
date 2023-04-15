﻿
import ReactDom from 'react-dom';

export const DialogueBox = ({ setShowModal, product }) => {



    return ReactDom.createPortal(
        <div className="modal" id={"cartCounter" + product.productId} tabIndex="-1">
            <div className="modal-dialog">
                <div className="modal-content">
                    <div className="modal-header">
                        <h5 className="modal-title">Modal title</h5>
                        <button type="button"
                            className="btn-close"
                            data-bs-dismiss="modal"
                            aria-label="Close"
                        ></button>
                    </div>
                    <div className="modal-body">
                        <p>Modal body text goes here.</p>
                    </div>
                    <div className="modal-footer">
                        <button type="button"
                            className="btn btn-secondary"
                            data-bs-dismiss="modal"
                        >Close</button>
                        <button type="button"
                            className="btn btn-primary"
                        >Save</button>
                    </div>
                </div>
            </div>
        </div>
        ,
        document.getElementById("portal")
    );
}
