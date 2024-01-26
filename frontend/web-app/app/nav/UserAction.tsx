'use client'

import { Dropdown } from 'flowbite-react'
import { User } from 'next-auth'
import { signOut } from 'next-auth/react'
import Link from 'next/link'
import React from 'react'
import { AiFillCar, AiFillTrophy, AiOutlineLogin, AiOutlineLogout } from 'react-icons/ai'
import { HiCog, HiUser } from 'react-icons/hi'

type Props = {
  user: Partial<User>
}

export default function UserAction({ user }: Props) {
  return (
    <Dropdown label={`Welcome ${user.name}`} inline>
      <Dropdown.Item icon={HiUser}>My Auctions</Dropdown.Item>
      <Dropdown.Item icon={AiFillTrophy}>Auction won</Dropdown.Item>
      <Dropdown.Item icon={AiFillCar}>Sell my car</Dropdown.Item>
      <Dropdown.Item icon={HiCog}>
        <Link href='/session'>
          Session (dev only)
        </Link>
      </Dropdown.Item>
      <Dropdown.Divider />
      <Dropdown.Item icon={AiOutlineLogout} onClick={() => signOut({ callbackUrl: '/' })}>Sign out</Dropdown.Item>
    </Dropdown>
  )
}
