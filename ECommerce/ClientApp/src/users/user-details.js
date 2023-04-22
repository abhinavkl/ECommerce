﻿import { useEffect } from "react"
import { useState } from "react"
import { useDispatch, useSelector } from "react-redux"
import { useHistory } from "react-router-dom"
import { fetchUserCartDetailsAsync, fetchUserDetailsAsync, fetchUserFavouritesDetailsAsync, fetchUserOrderDetailsAsync } from "../store/user-slice"
import { UserCard } from "./user-card"
import ProductCard from "../products/product-card"
import { DateItem } from "../products/orders"
import Pagination from "../shared/pagination"



export default function UserDetails(props) {

    const history = useHistory()
    const dispatch = useDispatch()

    const state = props.location.state
    const [userId] = useState(state?.userId)

    const { selectedUser }=useSelector(state=>state.user)
    const [dateRange,setDateRange]=useState('')

    useEffect(() => {
        dispatch(fetchUserDetailsAsync(userId))
        dispatch(fetchUserFavouritesDetailsAsync({ userId }))
        dispatch(fetchUserCartDetailsAsync({ userId }))
        dispatch(fetchUserOrderDetailsAsync({ userId, dateFilter: dateRange }))
    }, [userId, dispatch])


    return (
        <div className="row">
            <div className="col-2">
                <div className="row">
                    <div className="col text-center border border-primary m-2">
                        <UserCard user={selectedUser.user}></UserCard>
                    </div>
                </div>
            </div>
            <div className="col-10">
                <ul className="nav nav-tabs" id="myTab" role="tablist">
                    <li className="nav-item" role="presentation">
                        <button className="nav-link active"
                            id="fav-tab"
                            data-bs-toggle="tab"
                            data-bs-target="#fav-tab-pane"
                            type="button" role="tab"
                            aria-controls="fav-tab-pane"
                            aria-selected="true">Favourites</button>
                    </li>
                    <li className="nav-item" role="presentation">
                        <button className="nav-link"
                            id="cart-tab"
                            data-bs-toggle="tab"
                            data-bs-target="#cart-tab-pane"
                            type="button"
                            role="tab"
                            aria-controls="cart-tab-pane"
                            aria-selected="false">Cart</button>
                    </li>
                    <li className="nav-item" role="presentation">
                        <button
                            className="nav-link"
                            id="order-tab"
                            data-bs-toggle="tab"
                            data-bs-target="#order-tab-pane"
                            type="button"
                            role="tab"
                            aria-controls="order-tab-pane"
                            aria-selected="false">Orders</button>
                    </li>
                </ul>
                <div className="tab-content" id="myTabContent">
                    <div className="tab-pane fade show active"
                        id="fav-tab-pane"
                        role="tabpanel"
                        aria-labelledby="fav-tab"
                        tabIndex="0">
                        <div className="row pt-4">
                            <Pagination pageNumber={selectedUser.favourites.pageNumber}
                                totalPages={selectedUser.favourites.totalPages}
                                setPage={(page) => dispatch(fetchUserFavouritesDetailsAsync({ userId, pageNumber: page }))} >
                            </Pagination>
                        </div>
                        <div className="row">
                            {selectedUser && selectedUser.favourites && selectedUser.favourites.result.map(product =>
                                <ProductCard key={product.productId} product={product}>
                                </ProductCard>
                            )}
                            {(!selectedUser || !selectedUser.favourites || selectedUser.favourites.result.length === 0) &&
                                <>
                                    <div className="col p-4 fw-bold fs-4" style={{ color: 'orange' }}>
                                        No product added to favourites.
                                    </div>
                                </>
                            }
                        </div>
                    </div>
                    <div className="tab-pane fade"
                        id="cart-tab-pane"
                        role="tabpanel"
                        aria-labelledby="cart-tab"
                        tabIndex="0">
                        <div className="row pt-4">
                            <Pagination pageNumber={selectedUser.cart.pageNumber}
                                totalPages={selectedUser.cart.totalPages}
                                setPage={(page) => dispatch(fetchUserCartDetailsAsync({ userId, pageNumber: page }))} >
                            </Pagination>
                        </div>
                        <div className="row">
                            {selectedUser && selectedUser.cart && selectedUser.cart.result.map(product =>
                                <ProductCard key={product.productId} product={product}>
                                </ProductCard>
                            )}
                            {(!selectedUser || !selectedUser.cart || selectedUser.cart.result.length === 0) &&
                                <>
                                    <div className="col p-4 fw-bold fs-4" style={{ color: 'orange' }}>
                                        No product added to cart.
                                    </div>
                                </>
                            }
                        </div>
                    </div>
                    <div className="tab-pane fade"
                        id="order-tab-pane"
                        role="tabpanel"
                        aria-labelledby="order-tab"
                        tabIndex="0">
                        <div className="row mt-4 align-items-center">
                            <div className="col-2 text-end">
                                <label className="fw-bold">Range :</label>
                            </div>
                            <div className="col-3">
                                <select className="form-control"
                                    value={dateRange}
                                    onChange={(event) => setDateRange(event.target.value)}
                                >
                                    <option value="30">Last 30 days</option>
                                    <option value="90">Last 90 days</option>
                                    <option value="180">Last 180 days</option>
                                    <option value="365">Last 365 days</option>
                                </select>
                            </div>
                            <div className="col-2">
                                <button type="button"
                                    className="btn btn-primary"
                                    onClick={() => dispatch(fetchUserOrderDetailsAsync({ userId, dateFilter: dateRange }))}
                                >
                                    Update
                                </button>
                            </div>
                        </div>
                        <div className="row">
                            {selectedUser && selectedUser.orders && selectedUser.orders.map(order =>
                                <DateItem key={order.date} order={order}></DateItem>
                            )}
                            {(!selectedUser || !selectedUser.orders || selectedUser.orders.length === 0) &&
                                <>
                                    <div className="col p-4 fw-bold fs-4" style={{ color: 'orange' }}>
                                        No product added to cart.
                                    </div>
                                </>
                            }

                        </div>
                    </div>
                </div>
            </div>
        </div>
    )
}
