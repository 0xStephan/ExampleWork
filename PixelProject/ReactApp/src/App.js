import "./App.css";
//import { ConnectButton } from "@rainbow-me/rainbowkit";
import contractInterface from "../src/contract-abi.json";
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
import {
  ConnectKitProvider,
  ConnectKitButton,
  getDefaultClient,
} from "connectkit";
import { Buffer } from "buffer";

Buffer.from("anything", "base64");
window.Buffer = window.Buffer || require("buffer").Buffer;

function App() {
  function connectedHeight() {
    window.top.postMessage("Height " + document.body.scrollHeight, "*");
  }

  const CONTRACT_ADDRESS = "0x236F978bBe887fFaBF0612cF0EAF165b7749e146";
  const GOLD_PRICE = "0.22";
  const PLATINUM_PRICE = "0.33";
  const DIAMOND_PRICE = "0.44";
  const TRIPLEA_PRICE = "100";

  /*
  const { data, isLoading, error } = useContractRead({
    addressOrName: CONTRACT_ADDRESS,
    contractInterface: contractInterface,
    functionName: "getPrice",
    args: "1",
  });
  */

  const { address, isConnected, isDisconnected } = useAccount();

  // Whitelist only
  const {
    data: dataWhitelistOnly,
    isLoading: loadingWhitelistOnly,
    error: errorWhitelistOnly,
  } = useContractRead({
    addressOrName: CONTRACT_ADDRESS,
    contractInterface: contractInterface,
    functionName: "whitelistOnly",
    watch: true,
  });

  // Remove checks for dataWhitelisted to remove whitelist check

  // Whitelisted mapping
  const {
    data: dataWhitelisted,
    isLoading: loadingWhitelisted,
    error: errorWhitelisted,
  } = useContractRead({
    addressOrName: CONTRACT_ADDRESS,
    contractInterface: contractInterface,
    functionName: "whitelisted",
    watch: true,
    args: [address],
  });

  // WhitelistOnly
  const {
    data: dataWhiteOnly,
    isLoading: loadingWhiteOnly,
    error: errorWhiteOnly,
  } = useContractRead({
    addressOrName: CONTRACT_ADDRESS,
    contractInterface: contractInterface,
    functionName: "whitelistOnly",
    watch: true,
  });

  // Mint available
  const {
    data: dataMintEnabled,
    isLoading: loadingMintEnabled,
    error: errorMintEnabled,
  } = useContractRead({
    addressOrName: CONTRACT_ADDRESS,
    contractInterface: contractInterface,
    functionName: "mintEnabled",
    watch: true,
  });

  // Available variables
  const {
    data: dataGoldAvailable,
    isLoading: loadingGoldAvailable,
    error: errorGoldAvailable,
  } = useContractRead({
    addressOrName: CONTRACT_ADDRESS,
    contractInterface: contractInterface,
    functionName: "goldAvailable",
    watch: true,
  });

  const {
    data: dataPlatinumAvailable,
    isLoading: loadingPlatinumAvailable,
    error: errorPlatinumAvailable,
  } = useContractRead({
    addressOrName: CONTRACT_ADDRESS,
    contractInterface: contractInterface,
    functionName: "platinumAvailable",
    watch: true,
  });

  const {
    data: dataDiamondAvailable,
    isLoading: loadingDiamondAvailable,
    error: errorDiamondAvailable,
  } = useContractRead({
    addressOrName: CONTRACT_ADDRESS,
    contractInterface: contractInterface,
    functionName: "diamondAvailable",
    watch: true,
  });

  const {
    data: dataTripleaAvailable,
    isLoading: loadingTripleaAvailable,
    error: errorTripleaAvailable,
  } = useContractRead({
    addressOrName: CONTRACT_ADDRESS,
    contractInterface: contractInterface,
    functionName: "tripleaAvailable",
  });

  // Supply variables
  const {
    data: dataGoldSupply,
    isLoading: loadingGoldSupply,
    error: errorGoldSupply,
  } = useContractRead({
    addressOrName: CONTRACT_ADDRESS,
    contractInterface: contractInterface,
    functionName: "goldSupply",
    watch: true,
  });

  const {
    data: dataPlatinumSupply,
    isLoading: loadingPlatinumSupply,
    error: errorPlatinumSupply,
  } = useContractRead({
    addressOrName: CONTRACT_ADDRESS,
    contractInterface: contractInterface,
    functionName: "platinumSupply",
    watch: true,
  });

  const {
    data: dataDiamondSupply,
    isLoading: loadingDiamondSupply,
    error: errorDiamondSupply,
  } = useContractRead({
    addressOrName: CONTRACT_ADDRESS,
    contractInterface: contractInterface,
    functionName: "diamondSupply",
    watch: true,
  });

  const {
    data: dataTripleaSupply,
    isLoading: loadingTripleaSupply,
    error: errorTripleaSupply,
  } = useContractRead({
    addressOrName: CONTRACT_ADDRESS,
    contractInterface: contractInterface,
    functionName: "tripleaSupply",
    watch: true,
  });

  // Gold mint
  const { config: configGold, error: errorGold } = usePrepareContractWrite({
    addressOrName: CONTRACT_ADDRESS,
    contractInterface: contractInterface,
    functionName: "mint",
    args: ["0"],
    overrides: { value: ethers.utils.parseEther(GOLD_PRICE) },
  });
  const {
    data: mintDataGold,
    isLoading: isMintLoadingGold,
    isSuccess: isMintStartedGold,
    write: mintGold,
  } = useContractWrite(configGold);

  //Platinum mint
  const { config: configPlatinum, error: errorPlatinum } =
    usePrepareContractWrite({
      addressOrName: CONTRACT_ADDRESS,
      contractInterface: contractInterface,
      functionName: "mint",
      args: ["1"],
      overrides: { value: ethers.utils.parseEther(PLATINUM_PRICE) },
    });
  const {
    data: mintDataPlatinum,
    isLoading: isMintLoadingPlatinum,
    isSuccess: isMintStartedPlatinum,
    write: mintPlatinum,
  } = useContractWrite(configPlatinum);

  // Diamond mint
  const { config: configDiamond, error: errorDiamond } =
    usePrepareContractWrite({
      addressOrName: CONTRACT_ADDRESS,
      contractInterface: contractInterface,
      functionName: "mint",
      args: ["2"],
      overrides: { value: ethers.utils.parseEther(DIAMOND_PRICE) },
    });
  const {
    data: mintDataDiamond,
    isLoading: isMintLoadingDiamond,
    isSuccess: isMintStartedDiamond,
    write: mintDiamond,
  } = useContractWrite(configDiamond);

  /*
  // Triplea mint
  const { config: configTriplea, error: errorTriplea } =
    usePrepareContractWrite({
      addressOrName: CONTRACT_ADDRESS,
      contractInterface: contractInterface,
      functionName: "mint",
      args: ["3"],
      overrides: { value: ethers.utils.parseEther(TRIPLEA_PRICE) },
    });
  const {
    data: mintDataTriplea,
    isLoading: isMintLoadingTriplea,
    isSuccess: isMintStartedTriplea,
    write: mintTriplea,
  } = useContractWrite(configTriplea);
  */

  // Track gold mint
  const { isSuccess: txSuccessGold } = useWaitForTransaction({
    hash: mintDataGold?.hash,
  });

  const isMintedGold = txSuccessGold;

  // Track platinum mint
  const { isSuccess: txSuccessPlatinum } = useWaitForTransaction({
    hash: mintDataPlatinum?.hash,
  });

  const isMintedPlatinum = txSuccessPlatinum;

  // Track diamond mint
  const { isSuccess: txSuccessDiamond } = useWaitForTransaction({
    hash: mintDataDiamond?.hash,
  });

  const isMintedDiamond = txSuccessDiamond;

  /*
  // Track triplea mint
  const { isSuccess: txSuccessTriplea } = useWaitForTransaction({
    hash: mintDataTriplea?.hash,
  });

  const isMintedTriplea = txSuccessTriplea;
  */

  return (
    <body>
      <header>
        <a href="https://pixelboss.io/club">
          <img
            src="https://pixelboss.io/wp-content/uploads/2022/07/pxb-club-logo-default.svg"
            className="header header-img"
          ></img>
        </a>
      </header>
      <div className="main">
        <div className="gradient">
          <div className="banner">
            {!isConnected && <p className="big-text">Connect Wallet</p>}
            {isConnected && (
              <div
                className="contents"
                style={{ borderTop: "none", borderBottom: "none" }}
              >
                <div
                  className="inner"
                  style={{ borderLeft: "none", borderRight: "none" }}
                >
                  <div
                    className="row"
                    style={{ paddingTop: "0px", paddingBottom: "0px" }}
                  >
                    <div className="column small-text">
                      <ConnectKitButton />
                    </div>
                  </div>
                </div>
              </div>
            )}
          </div>
          <div className="contents">
            <div className="inner">
              {!isConnected && (
                <div className="row">
                  <div className="column">
                    <p className="big-text" onLoad={connectedHeight()}>
                      Welcome to the Pixel Boss Club.
                    </p>
                    <p className="big-text">Connect your wallet to continue.</p>
                    <ConnectKitButton.Custom>
                      {({
                        isConnected,
                        isConnecting,
                        show,
                        hide,
                        address,
                        ensName,
                      }) => {
                        return (
                          <button
                            onClick={show}
                            type="button"
                            className="connect-button big-text"
                            style={{ textAlign: "center" }}
                          >
                            {isConnected ? address : "Connect Wallet"}
                            <span>
                              <svg
                                xmlns="http://www.w3.org/2000/svg"
                                width="20"
                                height="18"
                                viewBox="0 0 20 18"
                                fill="none"
                                style={{ paddingLeft: "8px" }}
                                className="wallet-svg"
                              >
                                <path d="M17 4H16V3C16 2.20435 15.6839 1.44129 15.1213 0.87868C14.5587 0.316071 13.7956 0 13 0H3C2.20435 0 1.44129 0.316071 0.87868 0.87868C0.316071 1.44129 0 2.20435 0 3V15C0 15.7956 0.316071 16.5587 0.87868 17.1213C1.44129 17.6839 2.20435 18 3 18H17C17.7956 18 18.5587 17.6839 19.1213 17.1213C19.6839 16.5587 20 15.7956 20 15V7C20 6.20435 19.6839 5.44129 19.1213 4.87868C18.5587 4.31607 17.7956 4 17 4ZM3 2H13C13.2652 2 13.5196 2.10536 13.7071 2.29289C13.8946 2.48043 14 2.73478 14 3V4H3C2.73478 4 2.48043 3.89464 2.29289 3.70711C2.10536 3.51957 2 3.26522 2 3C2 2.73478 2.10536 2.48043 2.29289 2.29289C2.48043 2.10536 2.73478 2 3 2ZM18 12H17C16.7348 12 16.4804 11.8946 16.2929 11.7071C16.1054 11.5196 16 11.2652 16 11C16 10.7348 16.1054 10.4804 16.2929 10.2929C16.4804 10.1054 16.7348 10 17 10H18V12ZM18 8H17C16.2044 8 15.4413 8.31607 14.8787 8.87868C14.3161 9.44129 14 10.2044 14 11C14 11.7956 14.3161 12.5587 14.8787 13.1213C15.4413 13.6839 16.2044 14 17 14H18V15C18 15.2652 17.8946 15.5196 17.7071 15.7071C17.5196 15.8946 17.2652 16 17 16H3C2.73478 16 2.48043 15.8946 2.29289 15.7071C2.10536 15.5196 2 15.2652 2 15V5.83C2.32127 5.94302 2.65943 6.00051 3 6H17C17.2652 6 17.5196 6.10536 17.7071 6.29289C17.8946 6.48043 18 6.73478 18 7V8Z"></path>
                              </svg>
                            </span>
                          </button>
                        );
                      }}
                    </ConnectKitButton.Custom>
                  </div>
                </div>
              )}
              <div>
                {isConnected &&
                  !(
                    isMintStartedGold ||
                    isMintStartedPlatinum ||
                    isMintStartedDiamond
                  ) &&
                  !(isMintedGold || isMintedPlatinum || isMintedDiamond) && (
                    <div>
                      <div className="row">
                        <p className="big-text" onLoad={connectedHeight()}>
                          Which membership will you choose?
                        </p>
                      </div>
                    </div>
                  )}
                {isConnected &&
                  (isMintStartedGold ||
                    isMintStartedPlatinum ||
                    isMintStartedDiamond) &&
                  !(isMintedGold || isMintedPlatinum || isMintedDiamond) && (
                    <div className="row">
                      <div className="column">
                        <p className="big-text">You're almost there...</p>{" "}
                        {isMintStartedGold &&
                          !(
                            isMintedGold ||
                            isMintedDiamond ||
                            isMintedPlatinum
                          ) && (
                            <span>
                              <p>Minting your Gold Boss membership.</p>
                              <a
                                href={`https://etherscan.io/tx/${mintDataGold?.hash}`}
                                target="_blank"
                              >
                                View on Etherscan
                              </a>
                            </span>
                          )}
                        {isMintStartedPlatinum &&
                          !(
                            isMintedGold ||
                            isMintedDiamond ||
                            isMintedPlatinum
                          ) && (
                            <span>
                              <p>Minting your Platinum Boss membership.</p>
                              <a
                                href={`https://etherscan.io/tx/${mintDataPlatinum?.hash}`}
                                target="_blank"
                              >
                                View on Etherscan
                              </a>
                            </span>
                          )}
                        {isMintStartedDiamond &&
                          !(
                            isMintedGold ||
                            isMintedDiamond ||
                            isMintedPlatinum
                          ) && (
                            <span>
                              <p>Minting your Diamond Boss membership.</p>
                              <a
                                href={`https://etherscan.io/tx/${mintDataDiamond?.hash}`}
                                target="_blank"
                              >
                                View on Etherscan
                              </a>
                            </span>
                          )}
                      </div>
                      <div
                        className="column"
                        style={{ paddingLeft: "40px", paddingRight: "40px" }}
                      >
                        <div className="dot-pulse"></div>
                      </div>
                    </div>
                  )}
                {isConnected &&
                  (isMintedGold || isMintedPlatinum || isMintedDiamond) && (
                    <div className="row">
                      <div className="column">
                        <p className="big-text">
                          You are now part of the Pixel Boss Club!
                        </p>
                        {isMintedGold && (
                          <span>
                            <p>
                              Successfully minted your Gold Boss membership.
                            </p>
                            <a
                              href={`https://etherscan.io/tx/${mintDataGold?.hash}`}
                              target="_blank"
                            >
                              View on Etherscan
                            </a>
                            <p></p>
                            <a
                              href={`https://opensea.io/assets/ethereum/${configGold.addressOrName}/0`}
                              target="_blank"
                            >
                              View on Opensea
                            </a>
                          </span>
                        )}
                        {isMintedPlatinum && (
                          <span>
                            <p>
                              Successfully minted your Platinum Boss membership.
                            </p>
                            <a
                              href={`https://etherscan.io/tx/${mintDataPlatinum?.hash}`}
                              target="_blank"
                            >
                              View on Etherscan
                            </a>
                            <p></p>
                            <a
                              href={`https://opensea.io/assets/ethereum/${configPlatinum.addressOrName}/1`}
                              target="_blank"
                            >
                              View on Opensea
                            </a>
                          </span>
                        )}
                        {isMintedDiamond && (
                          <span>
                            <p>
                              Successfully minted your Diamond Boss membership.
                            </p>
                            <a
                              href={`https://etherscan.io/tx/${mintDataDiamond?.hash}`}
                              target="_blank"
                            >
                              View on Etherscan
                            </a>
                            <p></p>
                            <a
                              href={`https://opensea.io/assets/ethereum/${configDiamond.addressOrName}/2`}
                              target="_blank"
                            >
                              View on Opensea
                            </a>
                          </span>
                        )}
                      </div>
                      <div className="column">
                        {isMintedGold && (
                          <img
                            src="https://pixelboss.io/wp-content/uploads/2022/09/gold-GIF.gif"
                            className="responsive"
                          />
                        )}
                        {isMintedPlatinum && (
                          <img
                            src="https://pixelboss.io/wp-content/uploads/2022/09/platinum-GIF.gif"
                            className="responsive"
                          />
                        )}
                        {isMintedDiamond && (
                          <img
                            src="https://pixelboss.io/wp-content/uploads/2022/09/diamond-GIF.gif"
                            className="responsive"
                          />
                        )}
                      </div>
                    </div>
                  )}
                <p></p>
              </div>
            </div>
          </div>
        </div>
        {isConnected &&
          !(
            isMintStartedGold ||
            isMintStartedPlatinum ||
            isMintStartedDiamond
          ) && (
            <div className="NFT-Tiers" onLoad={() => connectedHeight()}>
              <div className="contents-black">
                <div className="inner">
                  <div>
                    <div className="row-nft">
                      <div className="column">
                        <img
                          src="https://raw.githubusercontent.com/0xStephan/pixeltest/main/gold-card.png"
                          className="membership-card"
                        />
                      </div>
                      <div className="column">
                        <p className="big-text">Gold Boss - {GOLD_PRICE} ETH</p>
                        {!loadingGoldAvailable &&
                          !errorGoldAvailable &&
                          !loadingGoldSupply &&
                          !errorGoldSupply && (
                            <p className="small-text">
                              {dataGoldSupply / 1} / {dataGoldAvailable / 1}{" "}
                              minted
                            </p>
                          )}
                      </div>
                      <div className="column">
                        {isMintLoadingGold && (
                          <button className="approval-button">
                            Waiting for approval
                          </button>
                        )}
                        {!errorGold &&
                          (dataWhitelisted || !dataWhitelistOnly) &&
                          !isMintLoadingGold &&
                          !isMintStartedGold && (
                            <button
                              className="mint-button"
                              onClick={() => mintGold?.()}
                              disabled={isMintLoadingGold || isMintStartedGold}
                              data-mint-loading={isMintLoadingGold}
                              data-mint-started={isMintStartedGold}
                            >
                              Mint Gold Boss NFT
                            </button>
                          )}

                        {errorGold &&
                          (dataWhitelisted || !dataWhitelistOnly) &&
                          dataMintEnabled &&
                          (errorGold.message?.toString() || "").includes(
                            "insufficient"
                          ) && (
                            <button className="error-button">
                              Insufficient Funds
                            </button>
                          )}

                        {!(dataWhitelisted || !dataWhitelistOnly) && (
                          <button className="error-button">
                            You are NOT whitelisted!
                          </button>
                        )}

                        {errorGold &&
                          (errorGold.message?.toString() || "").includes(
                            "available to mint"
                          ) && (
                            <button className="error-button">Sold out</button>
                          )}

                        {!dataMintEnabled &&
                          !loadingMintEnabled &&
                          !errorMintEnabled && (
                            <button className="error-button">
                              Mint not live yet!
                            </button>
                          )}

                        {errorGold &&
                          (errorGold.message?.toString() || "").includes(
                            "whitelist only"
                          ) && (
                            <button className="error-button">
                              Whitelist mint only
                            </button>
                          )}

                        {errorGold &&
                          (errorGold.message?.toString() || "").includes(
                            "already minted"
                          ) && (
                            <button className="error-button">
                              You have already minted an NFT!
                            </button>
                          )}
                      </div>
                    </div>
                  </div>
                </div>
              </div>
              <div className="contents-black">
                <div className="inner">
                  <div>
                    <div className="row-nft">
                      <div className="column">
                        <img
                          src="https://raw.githubusercontent.com/0xStephan/pixeltest/main/platinum-card.png"
                          className="membership-card"
                        />
                      </div>
                      <div className="column">
                        <p className="big-text">
                          Platinum Boss - {PLATINUM_PRICE} ETH
                        </p>
                        {!loadingPlatinumAvailable &&
                          !errorPlatinumAvailable &&
                          !loadingPlatinumSupply &&
                          !errorPlatinumSupply && (
                            <p className="small-text">
                              {dataPlatinumSupply / 1} /{" "}
                              {dataPlatinumAvailable / 1} minted
                            </p>
                          )}
                      </div>
                      <div className="column">
                        {isMintLoadingPlatinum && (
                          <button className="approval-button">
                            Waiting for approval
                          </button>
                        )}
                        {!errorPlatinum &&
                          (dataWhitelisted || !dataWhitelistOnly) &&
                          !isMintLoadingPlatinum &&
                          !isMintStartedPlatinum && (
                            <button
                              className="mint-button"
                              onClick={() => mintPlatinum?.()}
                              disabled={
                                isMintLoadingPlatinum || isMintStartedPlatinum
                              }
                              data-mint-loading={isMintLoadingPlatinum}
                              data-mint-started={isMintStartedPlatinum}
                            >
                              Mint Platinum Boss NFT
                            </button>
                          )}

                        {errorPlatinum &&
                          (dataWhitelisted || !dataWhitelistOnly) &&
                          dataMintEnabled &&
                          (errorPlatinum.message?.toString() || "").includes(
                            "insufficient"
                          ) && (
                            <button className="error-button">
                              Insufficient Funds
                            </button>
                          )}

                        {!(dataWhitelisted || !dataWhitelistOnly) && (
                          <button className="error-button">
                            You are NOT whitelisted!
                          </button>
                        )}

                        {errorPlatinum &&
                          (errorPlatinum.message?.toString() || "").includes(
                            "available to mint"
                          ) && (
                            <button className="error-button">Sold out</button>
                          )}

                        {!dataMintEnabled &&
                          !loadingMintEnabled &&
                          !errorMintEnabled && (
                            <button className="error-button">
                              Mint not live yet!
                            </button>
                          )}

                        {errorPlatinum &&
                          (errorPlatinum.message?.toString() || "").includes(
                            "whitelist only"
                          ) && (
                            <button className="error-button">
                              Whitelist mint only
                            </button>
                          )}

                        {errorPlatinum &&
                          (errorPlatinum.message?.toString() || "").includes(
                            "already minted"
                          ) && (
                            <button className="error-button">
                              You have already minted an NFT!
                            </button>
                          )}
                      </div>
                    </div>
                  </div>
                </div>
              </div>
              <div className="contents-black">
                <div className="inner">
                  <div>
                    <div className="row-nft">
                      <div className="column">
                        <img
                          src="https://raw.githubusercontent.com/0xStephan/pixeltest/main/diamond-card.png"
                          className="membership-card"
                        />
                      </div>
                      <div className="column">
                        <p className="big-text">
                          Diamond Boss - {DIAMOND_PRICE} ETH
                        </p>
                        {!loadingDiamondAvailable &&
                          !errorDiamondAvailable &&
                          !loadingDiamondSupply &&
                          !errorDiamondSupply && (
                            <p className="small-text">
                              {dataDiamondSupply / 1} /{" "}
                              {dataDiamondAvailable / 1} minted
                            </p>
                          )}
                      </div>
                      <div className="column">
                        {isMintLoadingDiamond && (
                          <button className="approval-button">
                            Waiting for approval
                          </button>
                        )}
                        {!errorDiamond &&
                          (dataWhitelisted || !dataWhitelistOnly) &&
                          !isMintLoadingDiamond &&
                          !isMintStartedDiamond && (
                            <button
                              className="mint-button"
                              onClick={() => mintDiamond?.()}
                              disabled={
                                isMintLoadingDiamond || isMintStartedDiamond
                              }
                              data-mint-loading={isMintLoadingDiamond}
                              data-mint-started={isMintStartedDiamond}
                            >
                              Mint Diamond Boss NFT
                            </button>
                          )}

                        {errorDiamond &&
                          (dataWhitelisted || !dataWhitelistOnly) &&
                          dataMintEnabled &&
                          (errorDiamond.message?.toString() || "").includes(
                            "insufficient"
                          ) && (
                            <button className="error-button">
                              Insufficient Funds
                            </button>
                          )}

                        {!(dataWhitelisted || !dataWhitelistOnly) && (
                          <button className="error-button">
                            You are NOT whitelisted!
                          </button>
                        )}

                        {errorDiamond &&
                          (errorDiamond.message?.toString() || "").includes(
                            "available to mint"
                          ) && (
                            <button className="error-button">Sold out</button>
                          )}

                        {!dataMintEnabled &&
                          !loadingMintEnabled &&
                          !errorMintEnabled && (
                            <button className="error-button">
                              Mint not live yet!
                            </button>
                          )}

                        {errorDiamond &&
                          (errorDiamond.message?.toString() || "").includes(
                            "whitelist only"
                          ) && (
                            <button className="error-button">
                              Whitelist mint only
                            </button>
                          )}

                        {errorDiamond &&
                          (errorDiamond.message?.toString() || "").includes(
                            "already minted"
                          ) && (
                            <button className="error-button">
                              You have already minted an NFT!
                            </button>
                          )}
                      </div>
                    </div>
                  </div>
                </div>
              </div>
              <div className="contents">
                <div className="inner">
                  <div>
                    <div className="row-nft">
                      <div className="column">
                        <img
                          src="https://raw.githubusercontent.com/0xStephan/pixeltest/main/triplea-card.png"
                          className="membership-card"
                        />
                      </div>
                      <div className="column">
                        <p className="big-text">AAA Boss - Invite Only</p>
                        {!loadingTripleaAvailable &&
                          !errorTripleaAvailable &&
                          !loadingTripleaSupply &&
                          !errorTripleaSupply && (
                            <p className="small-text">
                              {dataTripleaSupply / 1} /{" "}
                              {dataTripleaAvailable / 1} minted
                            </p>
                          )}
                      </div>
                      <div className="column">
                        <button className="invite-button">Invite Only</button>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          )}
      </div>
      <footer>
        <div className="footer-text">
          <img
            src="https://pixelboss.io/wp-content/uploads/2022/08/mobile-menu-logo.svg"
            className="footer-img"
          ></img>
          © Pixel Boss, 2022{" "}
          <a href="https://dotf.com.au/" className="footer-text">
            Powered by DOTF
          </a>{" "}
        </div>
      </footer>
    </body>
  );
}

export default App;
