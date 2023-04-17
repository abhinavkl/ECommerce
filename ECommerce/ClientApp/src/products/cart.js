﻿import { useDispatch, useSelector } from "react-redux"
import { useEffect } from "react";
import ProductCard from "./product-card"
import Pagination from "../shared/pagination"
import { cartActions, getCartAsync } from "../store/cart-slice";
import { useHistory } from "react-router-dom";


export default function Cart() {

    const dispatch = useDispatch()
    const history=useHistory()
    const { products, pageNumber, totalPages, status } = useSelector(state => state.cart)

    useEffect(() => {
        dispatch(getCartAsync())
    }, [dispatch, pageNumber])

    function loadPage(page) {
        dispatch(cartActions.updatePageNumber(page))
    }

    function navigateOrderPage() {
        history.push('/confirm-order')        
    }

    return (
        <>
            <div className="row">
                <div className="col">
                    <div className="row">
                        {products.length!==0 && 
                            <div className="col-2">
                                <button
                                    type="button"
                                    className="btn btn-primary"
                                    onClick={navigateOrderPage}
                                >Go To Order Page
                                </button>
                            </div>
                        }
                        <div className="col text-end">
                            <Pagination pageNumber={pageNumber}
                                totalPages={totalPages}
                                setPage={loadPage} ></Pagination>
                        </div>
                    </div>
                    {status.isLoading &&
                        <div className={"row rowpad5px align-items-center "} >
                            <div className="col-8">
                                <div className={status.alertClass + " alert"} style={{ whiteSpace: "pre-wrap" }}>
                                    <div className={status.textClass}>{status.message}</div>
                                </div>
                            </div>
                        </div>
                    }
                    <div className="row">
                        {
                            products.map(product =>
                                <ProductCard key={product.productId} product={product}></ProductCard>
                            )
                        }
                    </div>
                </div>
            </div>
        </>
     )
}

