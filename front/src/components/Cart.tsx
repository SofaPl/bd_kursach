import React, { useEffect, useState } from 'react';
import axios from '../api';
import './Cart.css';
import { ItemCard } from '../types';

const ShoppingCart: React.FC = () => {
    const [cartItems, setCartItems] = useState<ItemCard[]>([]);
    const [totalPrice, setTotalPrice] = useState<number>(0);

    useEffect(() => {
        const fetchCartItems = async () => {
            const userId = localStorage.getItem('user_id');
            if (!userId) {
                alert('Please log in first');
                return;
            }

            try {
                const response = await axios.get(`/Items/user/${userId}/shoppingcart`);
                setCartItems(response.data);
                calculateTotalPrice(response.data);
            } catch (error) {
                console.error('Error fetching cart items', error);
            }
        };

        fetchCartItems();
    }, []);

    const calculateTotalPrice = (items: ItemCard[]) => {
        const total = items.reduce((sum, item) => sum + item.price, 0);
        setTotalPrice(total);
    };

    return (
        <div>
            <h1>Shopping Cart</h1>
            <div className="cart-grid">
                {cartItems.map((item) => (
                    <div key={item.productId} className="cart-item">
                        <img src={item.imageUrl} alt={item.title} className="cart-item-image" />
                        <h2>{item.title}</h2>
                        <p>{item.description}</p>
                        <p>{item.price} $</p>
                    </div>
                ))}
            </div>
            <h2>Total: {totalPrice} $</h2>
        </div>
    );
};

export default ShoppingCart;