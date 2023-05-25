"use client"
import { signIn } from 'next-auth/react';
import styles from './page.module.scss';

export default async function Index() {
  /*
   * Replace the elements below with your own.
   *
   * Note: The corresponding styles are in the ./index.scss file.
   */
  return (
    <div className={styles.page}>
      <div className="wrapper">
        <div className="container">
          <div id="welcome">
            <h1>
              <span> Hello there, </span>
              Welcome test-manager-research 👋
            </h1>
            <a
              className={styles.buttonPrimary}
              onClick={signInHandler}
            >
              Sign in
            </a>
          </div>
        </div>
      </div>
    </div>
  );
}

function signInHandler(e: { preventDefault: () => void; }) {
  e.preventDefault();
  signIn();
}

