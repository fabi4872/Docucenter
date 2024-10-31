// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

contract Stamper {
    struct Stamp {
        bytes32 object;
        address stamper;
        uint256 blockNo;
    }
    
    Stamp[] stampList;

    mapping(bytes32 => uint256[]) hashObjects;
    mapping(address => uint256[]) hashStampers;

    event Stamped(
        address indexed from,
        bytes32 indexed object,
        uint256 blockNo
    );

    address owner;

    constructor() {
        owner = msg.sender;

        // Inicializar con una entrada de prueba en la lista
        stampList.push(Stamp(0, msg.sender, block.number));
    }

    function put(bytes32[] memory objectList) public {
        uint256 max = objectList.length;
        for (uint256 i = 0; i < max; i++) {
            bytes32 object = objectList[i];
            uint256 newObjectIndex = stampList.length;
            stampList.push(Stamp(object, msg.sender, block.number));
            hashObjects[object].push(newObjectIndex);
            hashStampers[msg.sender].push(newObjectIndex);

            emit Stamped(msg.sender, object, block.number);
        }
    }

    function getStamplistPos(uint256 pos) public view returns (bytes32, address, uint256) {
        require(pos < stampList.length, "Posicion invalida");
        Stamp memory stamp = stampList[pos];
        return (stamp.object, stamp.stamper, stamp.blockNo);
    }

    function getObjectCount(bytes32 object) public view returns (uint256) {
        return hashObjects[object].length;
    }

    function getObjectPos(bytes32 object, uint256 pos) public view returns (uint256) {
        require(pos < hashObjects[object].length, "Posicion invalida");
        return hashObjects[object][pos];
    }

    function getBlockNo(bytes32 object, address stamper) public view returns (uint256) {
        uint256 length = hashObjects[object].length;
        for (uint256 i = 0; i < length; i++) {
            Stamp memory current = stampList[hashObjects[object][i]];
            if (current.stamper == stamper) {
                return current.blockNo;
            }
        }
        return 0;
    }

    function getStamperCount(address stamper) public view returns (uint256) {
        return hashStampers[stamper].length;
    }

    function getStamperPos(address stamper, uint256 pos) public view returns (uint256) {
        require(pos < hashStampers[stamper].length, "Posicion invalida");
        return hashStampers[stamper][pos];
    }
}