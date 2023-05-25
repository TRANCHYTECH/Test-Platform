"use client"
import { useSession } from 'next-auth/react';

/* eslint-disable-next-line */
export interface HeaderProps { }

export default function Header(props: HeaderProps) {
  const { data: session } = useSession({
    required: true
  });

  console.log('access token', session?.accessToken);

  return (
    <>
      <h1>Test Manager</h1>
      <p>Logged in user {session?.user.email}</p>
    </>
  );
}