import React from "react";
import ReactDOM from "react-dom/client";
import "./index.css";
import App from "./App";
import reportWebVitals from "./reportWebVitals";
/*
import "@rainbow-me/rainbowkit/styles.css";

import {
  RainbowKitProvider,
  wallet,
  darkTheme,
  connectorsForWallets,
} from "@rainbow-me/rainbowkit";
*/
import { chain, configureChains, createClient, WagmiConfig } from "wagmi";

import { publicProvider } from "wagmi/providers/public";
import { alchemyProvider } from "wagmi/providers/alchemy";

import {
  ConnectKitProvider,
  ConnectKitButton,
  getDefaultClient,
} from "connectkit";
import { WalletConnectConnector } from "wagmi/connectors/walletConnect";
import { MetaMaskConnector } from "wagmi/connectors/metaMask";
import { InjectedConnector } from "wagmi/connectors/injected";

/*
import { Buffer } from "buffer";
Buffer.from("anything", "base64");
window.Buffer = window.Buffer || require("buffer").Buffer;
*/

const alchemyId = "xm6VEgYpXLWHQdW0NwlqeJbxy1GNzWab";
const { chains, provider, webSocketProvider } = configureChains(
  [chain.mainnet],
  [
    alchemyProvider({
      apiKey: "fswISlpiJHEwJ5F8bFv8MwOkNH5rEDUw",
      priority: 0,
    }),
    publicProvider({ priority: 1 }),
  ]
);

const client = createClient({
  autoConnect: true,
  connectors: [
    new WalletConnectConnector({
      chains: chains,
      options: {
        qrcode: false,
      },
    }),
    new MetaMaskConnector({
      chains: chains,
      options: {
        qrcode: false,
      },
    }),
    new InjectedConnector({
      chains: chains,
      options: {
        qrcode: false,
      },
    }),
  ],
  provider,
  webSocketProvider,
});
/*
const { chains, provider } = configureChains(
  [chain.mainnet],
  [
    alchemyProvider({ alchemyId: process.env.REACT_APP_ALCHEMY }),
    publicProvider(),
  ]
);

const needsInjectedWalletFallback =
  typeof window !== "undefined" &&
  window.ethereum &&
  !window.ethereum.isMetaMask &&
  !window.ethereum.isCoinbaseWallet;

const connectors = connectorsForWallets([
  {
    groupName: "Recommended",
    wallets: [
      ...(needsInjectedWalletFallback
        ? [wallet.injected({ chains, shimDisconnect: true })]
        : [
            wallet.metaMask({ chains }),
            wallet.coinbase({ chains }),
            wallet.rainbow({ chains }),
            wallet.walletConnect({ chains }),
            wallet.trust({ chains }),
            wallet.ledger({ chains }),
          ]),
    ],
  },
]);


const wagmiClient = createClient({
  autoConnect: true,
  connectors,
  provider,
});
*/

const root = ReactDOM.createRoot(document.getElementById("root"));
root.render(
  <React.StrictMode>
    <WagmiConfig client={client}>
      <ConnectKitProvider theme="midnight">
        <App />
      </ConnectKitProvider>
    </WagmiConfig>
  </React.StrictMode>
);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
