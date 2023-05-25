import { Footer } from '../footer/footer';
import Header from '../header/header';
import Sidebar from '../sidebar/sidebar';

export default function VerticalLayout({ children }: { children: React.ReactNode; }) {
  return (
    <div className='container'>
      <div className='row'>
        <Header></Header>
      </div>
      <div className='row'>
        <div className='col'>
          <Sidebar></Sidebar>
        </div>
        <div className='col'>
          {children}
        </div>
      </div>
      <div className='row'>
        <Footer></Footer>
      </div>
    </div>
  );
}