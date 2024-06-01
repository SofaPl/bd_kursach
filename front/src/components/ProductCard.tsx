import React from 'react';
import { Card, CardMedia, CardContent, Typography, Button } from '@mui/material';
import { ItemCard } from '../types';
import axios from '../api';
import './ProductCard.css';

interface ProductCardProps {
    item: ItemCard;
}

const ProductCard: React.FC<ProductCardProps> = ({ item }) => {
    const handleAddToCart = async () => {
        const userId = localStorage.getItem('user_id');
        if (!userId) {
            alert('Please log in first');
            return;
        }

        try {
            await axios.post(`/Items/user/${userId}/shoppingcart`, { productId: item.productId });
            alert('Item added to cart');
        } catch (error) {
            console.error('Error adding item to cart', error);
        }
    };

    return (
        <Card className="product-card">
            <CardMedia
                component="img"
                height="140"
                image={item.imageUrl}
                alt={item.title}
            />
            <CardContent>
                <Typography gutterBottom variant="h5" component="div">
                    {item.title}
                </Typography>
                <Typography variant="body2" color="text.secondary">
                    {item.description}
                </Typography>
                <Typography variant="h6" color="text.primary">
                    ${item.price}
                </Typography>
                <Button variant="contained" color="primary" onClick={handleAddToCart}>
                    Add to Cart
                </Button>
            </CardContent>
        </Card>
    );
};

export default ProductCard;
