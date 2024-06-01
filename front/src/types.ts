// src/types.ts
export interface ItemCard {
    productId: number;
    title: string;
    price: number;
    description: string;
    category: string;
    imageUrl: string;
}

export interface User {
    userId: number;
    username: string;
    email: string;
    password: string;
    shoppingCart: number[];
}

export interface CardDetails {
    userId: number;
    cardNumber: string;
    cardholderName: string;
    expiryDate: string;
    cvv: string;
}