import React, { useEffect, useState } from 'react';
import axios from '../api';
import ProductCard from './ProductCard';
import { ItemCard } from '../types';
import { Container, Grid } from '@mui/material';
import './ProductGrid.css';

const ProductGrid: React.FC = () => {
    const [items, setItems] = useState<ItemCard[]>([]);
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const fetchItems = async () => {
            try {
                const response = await axios.get<ItemCard[]>('/items');
                setItems(response.data);
            } catch (err) {
                setError('Error loading data');
            } finally {
                setLoading(false);
            }
        };

        fetchItems();
    }, []);

    if (loading) {
        return <div>Loading...</div>;
    }

    if (error) {
        return <div>{error}</div>;
    }

    return (
        <Container>
            <h1>Product Catalog</h1>
            <Grid container spacing={3}>
                {items.map(item => (
                    <Grid item xs={12} sm={6} md={4} lg={3} key={item.productId}>
                        <ProductCard item={item} />
                    </Grid>
                ))}
            </Grid>
        </Container>
    );
};

export default ProductGrid;