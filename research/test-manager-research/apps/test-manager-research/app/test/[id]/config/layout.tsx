import Link from "next/link";
export default function TestSpecificLayout({
  children,
  params
}: {
  children: React.ReactNode,
  params: { id: string }
}) {
  return (
    <>
      <div>
        <div>
          <h3>Test Specific</h3>
          <Link href={`/test/${params.id}/config/basic-settings`}>Basic Settings</Link>
          <Link href={`/test/${params.id}/config/times`}>Times</Link>
        </div>
        <div>{children}</div>
      </div>
    </>
  );
}
