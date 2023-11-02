import React from "react";
import ReactDOM from "react-dom/client";
import "./index.css";
import App from "./App";
import reportWebVitals from "./reportWebVitals";
import "@rainbow-me/rainbowkit/styles.css";
import {
  RainbowKitProvider,
  wallet,
  midnightTheme,
  darkTheme,
  connectorsForWallets,
} from "@rainbow-me/rainbowkit";
import { chain, configureChains, createClient, WagmiConfig } from "wagmi";
import { jsonRpcProvider } from "wagmi/providers/jsonRpc";
import { publicProvider } from "wagmi/providers/public";

const dogeChain: Chain = {
  id: 20_00,
  name: "Dogechain",
  network: "Dogechain",
  iconUrl: "https://dogechain.dog/favicon.ico",
  iconBackground: "#fff",
  nativeCurrency: {
    decimals: 18,
    name: "Dogechain",
    symbol: "DOGE",
  },
  rpcUrls: {
    default: "https://rpc01-sg.dogechain.dog/",
  },
  blockExplorers: {
    default: { name: "SnowTrace", url: "https://explorer.dogechain.dog" },
    etherscan: { name: "SnowTrace", url: "https://explorer.dogechain.dog" },
  },
  testnet: false,
};

const { chains, provider } = configureChains(
  [dogeChain],
  [jsonRpcProvider({ rpc: (chain) => ({ http: chain.rpcUrls.default }) })]
);

const connectors = connectorsForWallets([
  {
    groupName: "Recommended",
    wallets: [
      wallet.metaMask({ chains }),
      wallet.walletConnect({ chains }),
      wallet.trust({ chains }),
      wallet.ledger({ chains }),
    ],
  },
]);

const wagmiClient = createClient({
  autoConnect: true,
  connectors,
  provider,
});

const root = ReactDOM.createRoot(document.getElementById("root"));
root.render(
  <React.StrictMode>
    <WagmiConfig client={wagmiClient}>
      <RainbowKitProvider chains={chains} theme={darkTheme()}>
        <App />
      </RainbowKitProvider>
    </WagmiConfig>
  </React.StrictMode>
);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
