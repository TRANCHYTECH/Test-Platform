
import './global.scss';
import Provider from './_components/provider';
import VerticalLayout from './_components/vertical-layout/vertical-layout';
import 'bootstrap/dist/css/bootstrap.css';

export default function RootLayout({
  children
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="en">
      <body>
        <Provider>
          <VerticalLayout>{children}</VerticalLayout> 
        </Provider>
      </body>
    </html>
  );
}

// https://next-auth.js.org/configuration/initialization#route-handlers-app
// https://www.youtube.com/watch?v=Eh3EpwqT4cM&ab_channel=HamedBahram
