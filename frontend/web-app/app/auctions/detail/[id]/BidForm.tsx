'use client'

type Props = {
    auctionId: string,
    highBid: number
}

import { placeBidForAuction } from '@/app/actions/auctionAction'
import { numberWithCommas } from '@/app/lib/numberWithComma'
import { useBidStore } from '@/hooks/useBidStore'
import React from 'react'
import { FieldValues, useForm } from 'react-hook-form'

export default function BidForm({ auctionId, highBid }: Props) {
    const { register, handleSubmit, reset, formState: { errors } } = useForm();
    const addBids = useBidStore(state => state.addBid);

    function onSubmit(data: FieldValues) {
        placeBidForAuction(auctionId, +data.amount)
            .then((bid) => {
                addBids(bid)
                reset();
            })
    }

    return (
        <form onSubmit={handleSubmit(onSubmit)} className='flex items-center border-2 rounded-lg py-2'>
            <input type='number' {...register('amount')}
                className='input-custom text-sm text-gray-600'
                placeholder={`Enter your bid (minimum bid is ${numberWithCommas(highBid + 1)})`} />
        </form>
    )
}
