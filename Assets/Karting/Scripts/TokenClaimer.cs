using System.Threading.Tasks;
using Web3sdks;
using UnityEngine;

public class TokenClaimer : MonoBehaviour
{
    private Web3sdksSDK sdk;

    public GameObject balanceText;

    public GameObject claimButton;

    async void OnEnable()
    {
        sdk =
            new Web3sdksSDK("optimism-goerli",
                new Web3sdksSDK.Options()
                {
                    gasless =
                        new Web3sdksSDK.GaslessOptions()
                        {
                            openzeppelin =
                                new Web3sdksSDK.OZDefenderOptions()
                                {
                                    relayerUrl =
                                        "https://api.defender.openzeppelin.com/autotasks/c2e9a6ca-f2e8-4521-926b-1f9daec2dcb8/runs/webhook/826a5b67-d55d-49dc-8651-5db958ba22b2/DPtceJtayVGgKSDejaFnWk"
                                }
                        }
                });

        Debug.Log(Web3.selectedWallet);

        // Connect wallet
        if (Web3.selectedWallet == "Metamask")
        {
            await Metamask();
        }
        if (Web3.selectedWallet == "Walletconnect")
        {
            await WalletConnect();
        }
        if (Web3.selectedWallet == "Coinbase")
        {
            await Coinbase();
        }
        CheckBalance();
    }

    public async void Claim()
    {
        // Update claim button text
        claimButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text =
            "Claiming...";

        await getTokenDrop().ERC20.Claim("25");

        // hide claim button
        claimButton.SetActive(false);

        CheckBalance();
    }

    private Contract getTokenDrop()
    {
        return sdk.GetContract("0x4a9659d5E0d416Ce8B9a4336132012Af8db4c5AB");
    }

    private async void CheckBalance()
    {
        // Set text to user's balance
        var bal = await getTokenDrop().ERC20.Balance();

        balanceText.GetComponent<TMPro.TextMeshProUGUI>().text =
            bal.displayValue + " " + bal.symbol;
    }

    public async Task<string> WalletConnect()
    {
        string address =
            await sdk
                .wallet
                .Connect(new WalletConnection()
                {
                    provider = WalletProvider.WalletConnect,
                    chainId = 420 // Switch the wallet Goerli network on connection
                });
        return address;
    }

    public async Task<string> Metamask()
    {
        string address =
            await sdk
                .wallet
                .Connect(new WalletConnection()
                {
                    provider = WalletProvider.MetaMask,
                    chainId = 420 // Switch the wallet Goerli network on connection
                });
        return address;
    }

    public async Task<string> Coinbase()
    {
        string address =
            await sdk
                .wallet
                .Connect(new WalletConnection()
                {
                    provider = WalletProvider.CoinbaseWallet,
                    chainId = 420 // Switch the wallet Goerli network on connection
                });
        return address;
    }
}
