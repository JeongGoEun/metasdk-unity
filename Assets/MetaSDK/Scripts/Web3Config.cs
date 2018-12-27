static class Web3Config
{
    // address example
    public const string url = "http://13.124.111.52:8545";
    public const string contractAddr = "0xcD181d3651EA3641607129c138eDA68631f63bFD";
    public const string abi = @"[
    {
        ""constant"": true,
        ""inputs"": [],
        ""name"": ""minScore"",
        ""outputs"": [
            {
                ""name"": """",
                ""type"": ""uint256""
            }
        ],
        ""payable"": false,
        ""stateMutability"": ""view"",
        ""type"": ""function""
    },
    {
        ""constant"": false,
        ""inputs"": [],
        ""name"": ""clearScore"",
        ""outputs"": [],
        ""payable"": false,
        ""stateMutability"": ""nonpayable"",
        ""type"": ""function""
    },
    {
        ""constant"": false,
        ""inputs"": [],
        ""name"": ""renounceOwnership"",
        ""outputs"": [],
        ""payable"": false,
        ""stateMutability"": ""nonpayable"",
        ""type"": ""function""
    },
    {
        ""constant"": true,
        ""inputs"": [],
        ""name"": ""owner"",
        ""outputs"": [
            {
                ""name"": """",
                ""type"": ""address""
            }
        ],
        ""payable"": false,
        ""stateMutability"": ""view"",
        ""type"": ""function""
    },
    {
        ""constant"": true,
        ""inputs"": [],
        ""name"": ""isOwner"",
        ""outputs"": [
            {
                ""name"": """",
                ""type"": ""bool""
            }
        ],
        ""payable"": false,
        ""stateMutability"": ""view"",
        ""type"": ""function""
    },
    {
        ""constant"": false,
        ""inputs"": [
            {
                ""name"": ""_userName"",
                ""type"": ""string""
            },
            {
                ""name"": ""_userScore"",
                ""type"": ""uint256""
            }
        ],
        ""name"": ""registerScore"",
        ""outputs"": [],
        ""payable"": false,
        ""stateMutability"": ""nonpayable"",
        ""type"": ""function""
    },
    {
        ""constant"": true,
        ""inputs"": [
            {
                ""name"": """",
                ""type"": ""uint256""
            }
        ],
        ""name"": ""rankMap"",
        ""outputs"": [
            {
                ""name"": ""userMetaId"",
                ""type"": ""address""
            },
            {
                ""name"": ""userName"",
                ""type"": ""string""
            },
            {
                ""name"": ""userScore"",
                ""type"": ""uint256""
            },
            {
                ""name"": ""timestamp"",
                ""type"": ""uint256""
            }
        ],
        ""payable"": false,
        ""stateMutability"": ""view"",
        ""type"": ""function""
    },
    {
        ""constant"": true,
        ""inputs"": [],
        ""name"": ""minIndex"",
        ""outputs"": [
            {
                ""name"": """",
                ""type"": ""uint256""
            }
        ],
        ""payable"": false,
        ""stateMutability"": ""view"",
        ""type"": ""function""
    },
    {
        ""constant"": false,
        ""inputs"": [
            {
                ""name"": ""newOwner"",
                ""type"": ""address""
            }
        ],
        ""name"": ""transferOwnership"",
        ""outputs"": [],
        ""payable"": false,
        ""stateMutability"": ""nonpayable"",
        ""type"": ""function""
    },
    {
        ""inputs"": [],
        ""payable"": false,
        ""stateMutability"": ""nonpayable"",
        ""type"": ""constructor""
    },
    {
        ""anonymous"": false,
        ""inputs"": [
            {
                ""indexed"": false,
                ""name"": ""_userName"",
                ""type"": ""string""
            },
            {
                ""indexed"": false,
                ""name"": ""_userScore"",
                ""type"": ""uint256""
            }
        ],
        ""name"": ""RegisterScore"",
        ""type"": ""event""
    },
    {
        ""anonymous"": false,
        ""inputs"": [
            {
                ""indexed"": true,
                ""name"": ""previousOwner"",
                ""type"": ""address""
            }
        ],
        ""name"": ""OwnershipRenounced"",
        ""type"": ""event""
    },
    {
        ""anonymous"": false,
        ""inputs"": [
            {
                ""indexed"": true,
                ""name"": ""previousOwner"",
                ""type"": ""address""
            },
            {
                ""indexed"": true,
                ""name"": ""newOwner"",
                ""type"": ""address""
            }
        ],
        ""name"": ""OwnershipTransferred"",
        ""type"": ""event""
    }
]";

}
