import "./App.css";
import contractInterface from "../src/contract-abi.json";
import { ConnectButton } from "@rainbow-me/rainbowkit";
import logo from "./logo.png";
import dogeheartmoving from "./doge-heart-moving.gif";
import { ethers } from "ethers";
import {
  useAccount,
  usePrepareContractWrite,
  useContractWrite,
  useContractRead,
  useBalance,
  useWaitForTransaction,
} from "wagmi";
import { useState } from "react";

function App() {
  const [qty, setQty] = useState(1);
  const [err, setErr] = useState("");

  const contractAddress = "0xD14DbD0782B941a082eCda57F83fE7Bea4BF4097";

  function incrementQty() {
    if (qty < 2 ** dataRound - dataDeposits) {
      setQty(qty + 1);
    }
  }

  function decrementQty() {
    if (qty > 1) {
      setQty(qty - 1);
    }
  }

  function maxQty() {
    setQty(2 ** dataRound - dataDeposits);
  }

  // Wagmi hooks
  const { address, isConnected, isDisconnected } = useAccount({
    watch: true,
  });

  const {
    data: balanceData,
    isError: balanceError,
    isLoading: balanceLoading,
  } = useBalance({
    addressOrName: address,
    watch: true,
  });

  const {
    data: dataLive,
    isLoading: loadingLive,
    error: errorLive,
  } = useContractRead({
    addressOrName: contractAddress,
    contractInterface: contractInterface,
    functionName: "getLive",
    watch: true,
  });

  const {
    data: dataRound,
    isLoading: loadingRound,
    error: errorRound,
  } = useContractRead({
    addressOrName: contractAddress,
    contractInterface: contractInterface,
    functionName: "getRound",
    watch: true,
  });

  const {
    data: dataDeposits,
    isLoading: loadingDeposits,
    error: errorDeposits,
  } = useContractRead({
    addressOrName: contractAddress,
    contractInterface: contractInterface,
    functionName: "getDeposits",
    watch: true,
  });

  const {
    data: dataFirstScore,
    isLoading: loadingFirstScore,
    error: errorFirstScore,
  } = useContractRead({
    addressOrName: contractAddress,
    contractInterface: contractInterface,
    functionName: "getFirstScore",
    watch: true,
  });

  const {
    data: dataSecondScore,
    isLoading: loadingSecondScore,
    error: errorSecondScore,
  } = useContractRead({
    addressOrName: contractAddress,
    contractInterface: contractInterface,
    functionName: "getSecondScore",
    watch: true,
  });

  const {
    data: dataThirdScore,
    isLoading: loadingThirdScore,
    error: errorThirdScore,
  } = useContractRead({
    addressOrName: contractAddress,
    contractInterface: contractInterface,
    functionName: "getThirdScore",
    watch: true,
  });

  const {
    data: dataFirstAddress,
    isLoading: loadingFirstAddress,
    error: errorFirstAddress,
  } = useContractRead({
    addressOrName: contractAddress,
    contractInterface: contractInterface,
    functionName: "getFirstAddress",
    watch: true,
  });

  const {
    data: dataSecondAddress,
    isLoading: loadingSecondAddress,
    error: errorSecondAddress,
  } = useContractRead({
    addressOrName: contractAddress,
    contractInterface: contractInterface,
    functionName: "getSecondAddress",
    watch: true,
  });

  const {
    data: dataThirdAddress,
    isLoading: loadingThirdAddress,
    error: errorThirdAddress,
  } = useContractRead({
    addressOrName: contractAddress,
    contractInterface: contractInterface,
    functionName: "getThirdAddress",
    watch: true,
  });

  const { config: myConfig, error: myError } = usePrepareContractWrite({
    addressOrName: contractAddress,
    contractInterface: contractInterface,
    functionName: "deposit",
    args: String(qty),
    overrides: { value: ethers.utils.parseEther(String(qty * 100)) },
  });

  const {
    data: enter,
    isLoading: enterLoading,
    isSuccess: enterSuccess,
    write: enterWrite,
  } = useContractWrite(myConfig);

  const { isSuccess: txSuccess } = useWaitForTransaction({
    hash: enter?.hash,
  });

  const isAccepted = txSuccess;

  return (
    <div className="header">
      <img src={logo} alt="Double Dog" style={{ marginTop: "20px" }}></img>
      <main>
        <div>
          <a
            href="https://twitter.com/DoubleDoge_xyz"
            target="_blank"
            className="small-text arcade-font"
            style={{ marginRight: "10px" }}
          >
            Twitter
          </a>
          <img
            src={dogeheartmoving}
            alt="much love"
            style={{ maxWidth: "100px", marginBottom: "20px" }}
          ></img>
          <a
            href="https://t.me/DoubleDogeBork"
            target="_blank"
            className="small-text arcade-font"
            style={{ marginLeft: "10px" }}
          >
            Telegram
          </a>
        </div>
        {!isConnected && (
          <div>
            <h1 className="arcade-font">
              Much <span className="highlight-purple">2x</span>
            </h1>
            <h1 className="arcade-font">
              {" "}
              such <span className="highlight-purple">WOW</span>
            </h1>
            <p className="arcade-font">Connect your wallet to continue</p>
            <h1>👇</h1>
          </div>
        )}
        <ConnectButton.Custom>
          {({
            account,
            chain,
            openAccountModal,
            openChainModal,
            openConnectModal,
            mounted,
          }) => {
            return (
              <div
                {...(!mounted && {
                  "aria-hidden": true,
                  style: {
                    opacity: 0,
                    pointerEvents: "none",
                    userSelect: "none",
                  },
                })}
              >
                {(() => {
                  if (!mounted || !account || !chain) {
                    return (
                      <button
                        onClick={openConnectModal}
                        type="button"
                        className="nes-btn arcade-font"
                        style={{
                          backgroundColor: "#7b6ffc",
                          color: "white",
                        }}
                      >
                        Connect Wallet
                      </button>
                    );
                  }

                  if (chain.unsupported) {
                    return (
                      <button
                        onClick={openChainModal}
                        type="button"
                        className="nes-btn arcade-font"
                        style={{
                          backgroundColor: "red",
                          color: "white",
                        }}
                      >
                        Wrong network
                      </button>
                    );
                  }

                  return (
                    <div style={{ display: "flex", gap: 12 }}>
                      <button
                        onClick={openAccountModal}
                        type="button"
                        className="nes-btn arcade-font"
                        style={{
                          backgroundColor: "#7b6ffc",
                          color: "white",
                          fontSize: "8px",
                        }}
                      >
                        {account.displayName}
                        {account.displayBalance
                          ? ` (${
                              (
                                balanceData?.value / 1000000000000000000
                              ).toFixed(4) +
                              " " +
                              balanceData?.symbol
                            })`
                          : ""}
                      </button>
                    </div>
                  );
                })()}
              </div>
            );
          }}
        </ConnectButton.Custom>
        {!isConnected && (
          <p
            className="small-text yellow-text questions"
            style={{ paddingTop: "10px", fontSize: "8px" }}
          >
            Please note that this dApp is for education and entertainment
            purposes only.
          </p>
        )}
        {isConnected && (loadingRound || loadingDeposits) && (
          <div style={{ paddingTop: "40px" }}>
            <p className="small-text">Loading data please wait... </p>
          </div>
        )}
        {isConnected &&
          dataLive &&
          !errorRound &&
          !loadingRound &&
          !errorDeposits &&
          !loadingDeposits && (
            <div style={{ paddingTop: "10px" }}>
              <p className="small-text">Round: {dataRound / 1}</p>
              <p className="small-text">
                <span className="green-text">Slots Filled:</span>{" "}
                {dataDeposits / 1}
              </p>
              <p className="small-text">
                <span className="red-text">Slots Remaining:</span>{" "}
                {2 ** dataRound - dataDeposits}
              </p>
              <p className="small-text">
                <span className="purple-text">Price per Slot:</span> 100 DOGE
              </p>
            </div>
          )}
        {isConnected && !dataLive && (
          <div>
            <p style={{ marginTop: "40px" }}>Not live yet. </p>
            <p>Please check telegram and twitter for launch details.</p>
          </div>
        )}
        <div className="horizontal">
          {isConnected &&
            dataLive &&
            !errorRound &&
            !loadingRound &&
            !errorDeposits &&
            !loadingDeposits && (
              <button
                type="button"
                className="nes-btn arcade-font"
                style={{
                  backgroundColor: "#7b6ffc",
                  color: "white",
                }}
                onClick={decrementQty}
              >
                -
              </button>
            )}
          {isConnected &&
            dataLive &&
            !errorRound &&
            !loadingRound &&
            !errorDeposits &&
            !loadingDeposits && (
              <button
                type="button"
                className="nes-btn arcade-font"
                style={{
                  backgroundColor: "white",
                  color: "black",
                }}
              >
                {qty}
              </button>
            )}
          {isConnected &&
            dataLive &&
            !errorRound &&
            !loadingRound &&
            !errorDeposits &&
            !loadingDeposits && (
              <button
                type="button"
                className="nes-btn arcade-font"
                style={{
                  backgroundColor: "#7b6ffc",
                  color: "white",
                }}
                onClick={incrementQty}
              >
                +
              </button>
            )}
          {isConnected &&
            dataLive &&
            !errorRound &&
            !loadingRound &&
            !errorDeposits &&
            !loadingDeposits && (
              <button
                type="button"
                className="nes-btn arcade-font"
                style={{
                  backgroundColor: "white",
                  color: "black",
                }}
                onClick={maxQty}
              >
                MAX
              </button>
            )}
        </div>
        <div style={{ paddingTop: "40px" }}>
          {myError &&
            dataLive &&
            (myError.message?.toString() || "").includes("insufficient") && (
              <button
                type="button"
                className="nes-btn arcade-font"
                style={{
                  backgroundColor: "red",
                  color: "white",
                  marginBottom: "20px",
                }}
              >
                Insufficient Funds
              </button>
            )}

          {myError &&
            dataLive &&
            (myError.message?.toString() || "").includes("QTY too large") && (
              <button
                type="button"
                className="nes-btn arcade-font"
                style={{
                  backgroundColor: "#FF6D0A",
                  color: "white",
                }}
              >
                Quantity too large. Please lower.
              </button>
            )}
          {isConnected &&
            dataLive &&
            !myError &&
            !enterLoading &&
            (!enterSuccess || isAccepted) &&
            !errorRound &&
            !loadingRound &&
            !errorDeposits &&
            !loadingDeposits && (
              <button
                type="button"
                className="nes-btn arcade-font"
                style={{
                  backgroundColor: "#7b6ffc",
                  color: "white",
                  marginBottom: "20px",
                }}
                onClick={() => enterWrite?.()}
              >
                Bork In
              </button>
            )}
          {isConnected &&
            dataLive &&
            enterLoading &&
            !enterSuccess &&
            !isAccepted && (
              <button
                type="button"
                className="nes-btn arcade-font"
                style={{
                  backgroundColor: "#FF6D0A",
                  color: "white",
                  marginBottom: "20px",
                }}
              >
                Waiting for Approval
              </button>
            )}
          {isConnected &&
            dataLive &&
            !enterLoading &&
            enterSuccess &&
            !isAccepted && (
              <button
                type="button"
                className="nes-btn arcade-font"
                style={{
                  backgroundColor: "#7CFB5C",
                  color: "black",
                  marginBottom: "20px",
                }}
              >
                Processing transaction. Please wait...
              </button>
            )}
        </div>
        {isAccepted && (
          <p className="small-text green-text">Much wow transaction success!</p>
        )}
        {isConnected &&
          dataLive &&
          !errorFirstAddress &&
          !loadingFirstAddress &&
          !errorSecondAddress &&
          !loadingSecondAddress &&
          !errorThirdAddress &&
          !loadingThirdAddress &&
          !errorFirstScore &&
          !loadingFirstScore &&
          !errorSecondScore &&
          !loadingSecondScore &&
          !errorThirdScore &&
          !loadingThirdScore && (
            <div
              className="nes-container is-dark with-title"
              style={{ paddingTop: "20px" }}
            >
              <p className="title">
                <i className="nes-icon trophy is-small"></i> Payout Highscore{" "}
                <i className="nes-icon trophy is-small"></i>
              </p>
              <p>
                1st: {dataFirstScore / 1000000000000000000} DOGE to{" "}
                {dataFirstAddress.toString().substring(0, 7)}
              </p>
              <p>
                2nd: {dataSecondScore / 1000000000000000000} DOGE to{" "}
                {dataSecondAddress.toString().substring(0, 7)}{" "}
              </p>
              <p>
                3rd: {dataThirdScore / 1000000000000000000} DOGE to{" "}
                {dataThirdAddress.toString().substring(0, 7)}{" "}
              </p>
            </div>
          )}
        <section className="message -left" style={{ marginTop: "20px" }}>
          <div className="nes-balloon from-left">
            <p style={{ color: "black" }} id="FAQ">
              Got questions?
            </p>
          </div>
          <div className="faq">
            <p className="arcade-font questions" style={{ marginTop: "40px" }}>
              How does this work?
            </p>
            <div className="answers">
              <p className="small-text gray-text">
                Slots deposited in the current round will be{" "}
                <span className="green-text">
                  paid out to users in the previous round
                </span>
                . This happens{" "}
                <span className="green-text">
                  once the current round has ended
                </span>
                . As each round has 2 times more slots than the previous round,
                you will receive 2 slots for every 1 slot you deposited in the
                previous round.
              </p>
              <p className="small-text gray-text">
                Round 1 has 2 slots. Round 2 has 4 slots, Round 3 has 8 slots,
                etc...
              </p>
            </div>
            <p className="arcade-font questions" style={{ marginTop: "40px" }}>
              How do I get into the highscore?
            </p>
            <div className="answers">
              <p className="small-text gray-text">
                Simply by recieving the highest payout in a single round. This
                is done by being the highest depositer in the previous round/s.
              </p>
            </div>
            <p className="arcade-font questions" style={{ marginTop: "40px" }}>
              Where can I view the transactions for this contract?
            </p>
            <div className="answers">
              <p className="small-text gray-text">
                You can view the contract on the explorer{" "}
                <a
                  href="https://explorer.dogechain.dog/address/0xd14dbd0782b941a082ecda57f83fe7bea4bf4097"
                  target="_blank"
                >
                  here.
                </a>
              </p>
            </div>
            <p className="arcade-font" style={{ marginTop: "40px" }}>
              Are there any fees?
            </p>
            <div className="answers">
              <p className="small-text gray-text">
                Yes currently 5% from each slot is allocated to the marketing
                and developer fund.
              </p>
            </div>
            <p className="arcade-font questions" style={{ marginTop: "40px" }}>
              Why didn't I get exactly a 2x?
            </p>
            <div className="answers">
              <p className="small-text gray-text">
                The smart contract takes a 5% tax from each slot. This is paid
                into the marketing and developer fund.
              </p>
            </div>
            <p className="arcade-font" style={{ marginTop: "40px" }}>
              Coming Soon...
            </p>
            <div className="answers">
              <i class="nes-logo"></i>
              <p className="small-text gray-text">P2E</p>
              <i class="nes-icon coin is-medium"></i>
              <p className="small-text gray-text">NFT Marketplace</p>
            </div>

            <p
              className="small-text yellow-text questions"
              style={{ marginTop: "40px" }}
            >
              Please note that this app is for education and entertainment
              purposes only.
            </p>
          </div>
        </section>
      </main>
    </div>
  );
}

export default App;
