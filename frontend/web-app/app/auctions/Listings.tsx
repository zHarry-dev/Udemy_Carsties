'use client'

import React, { useEffect, useState } from 'react'
import AuctionCard from './AuctionCard';
import AppPagination from '../components/AppPagination';
import { Auction, PageResult } from '@/types';
import { getData } from '../actions/auctionAction';
import Filters from './Filters';
import { useParamsStore } from '@/hooks/useParamsStore';
import { shallow } from 'zustand/shallow';
import qs from 'query-string';

export default function Listings() {
    const [data, setData] = useState<PageResult<Auction>>();
    const params = useParamsStore(state => ({
        pageNumber: state.pageNumber,
        pageSize: state.pageSize,
        searchTerm: state.searchTerm
    }), shallow);
    const setParams = useParamsStore(state => state.setParams)
    const url = qs.stringifyUrl({ url: '', query: params })

    function setPageNumber(pageNumber: number) {
        setParams({pageNumber})
    }

    useEffect(() => {
        getData(url).then(data => {
            setData(data);
        })
    }, [url])

    if (!data) return <h3>Loading...</h3>

    return (
        <>
            <Filters pageSize={params.pageSize} setPageSize={setPageSize} />
            <div className='grid grid-cols-4 gap-6'>
                {auctions.map(auction => (
                    <AuctionCard auction={auction} key={auction.id} />
                ))}
            </div>
            <div className='flex justify-center mt-4'>
                <AppPagination currentPage={pageNumber} pageCount={pageCount} pageChanged={setPageNumber} />
            </div>
        </>
    )
}