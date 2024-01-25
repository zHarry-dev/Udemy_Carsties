'use server'

import { Auction, PageResult } from "@/types";

export async function getData(pageNumber : number = 1, pageSize: number = 4): Promise<PageResult<Auction>> {
    const res = await fetch(`http://localhost:6001/search?pageSize=${pageSize}&pageNumber=${pageNumber}`);

    if (!res.ok) {
        throw new Error('Fail to get data');
    }

    return res.json();
}