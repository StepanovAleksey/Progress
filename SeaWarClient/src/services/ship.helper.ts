import { EGameStatus, type IShip } from "@/models/IShip.model";
import type { ICoordinateSimple } from "@/models/dto.model";
export class ShipHelper {
  constructor(
    private ships: Array<IShip>,
    private horizontalLength: number,
    private verticalLength: number,
    private maxRandomInterations = 50
  ) {}

  setRandomCoordinate() {
    const sortedShips = [...this.ships].sort(
      (a, b) => a.deltaCoordinates.length - b.deltaCoordinates.length
    );
    sortedShips.forEach((ship) => {
      ship.setNewCoordinate({
        x: -999,
        y: -999,
      });
    });
    while (sortedShips.length) {
      const maxedShip = sortedShips.pop();
      maxedShip.setStatus(EGameStatus.Move);
      let countShipSetPoth = 0;
      let randomCoordinate = this.getRandomCoordinate();
      while (!maxedShip.canMove(randomCoordinate, this.ships)) {
        randomCoordinate = this.getRandomCoordinate();
        countShipSetPoth++;
        if (countShipSetPoth > this.maxRandomInterations) {
          throw "Ошибка размещения корабля, превышенно кол-во попыток";
        }
      }
      maxedShip.setNewCoordinate(randomCoordinate);
      maxedShip.setStatus(EGameStatus.None);
    }
    return this.ships;
  }
  private getRandomCoordinate() {
    return <ICoordinateSimple>{
      x: this.getRandomInt(this.horizontalLength),
      y: this.getRandomInt(this.verticalLength),
    };
  }
  private getRandomInt(max: number) {
    return Math.floor(Math.random() * max);
  }
}
