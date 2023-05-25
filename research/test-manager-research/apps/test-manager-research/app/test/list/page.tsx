"use client"
import { useSession } from 'next-auth/react';
import styles from './page.module.scss';
import useSWR from 'swr';

export default function TestListPage() {
  const { data: session, status } = useSession({
    required: true
  });


  const fetcher = (url: string, token: string) => fetch(url, {
    method: 'GET',
    headers: {
      'Authorization': `Bearer ${token}`
    }
  }).then((res) => res.json());

  const { data, error, isLoading } = useSWR(status === 'authenticated' ? ['https://dev.test-manager-api.testmaster.io/Management/TestDefinition?pageNumber=1&pageSize=12', session?.accessToken ?? ''] : null , ([url, token]) => fetcher(url, token));

  if (error) return <div>failed to load</div>
  if (isLoading) return <div>loading...</div>
  return <div>{data && data.results.map(item => <li key={item.id}>{item.name}</li>) }</div>
}

