
export default async function BasicSettingsPage({ params }: { params: { id: string } }) {
  return (
    <div>
      <h1>Welcome to BasicSettingsPage! {params.id}</h1>
    </div>
  );
}

