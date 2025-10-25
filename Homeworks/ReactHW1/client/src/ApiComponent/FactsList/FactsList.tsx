import React from "react";
import './FactsList.css';

export interface IFact {
   fact: string;
   length: number;
}

interface IFactsProps {
   facts: IFact[];
}

const FactsList: React.FC<IFactsProps> = ({ facts }) => {
   if (!facts || facts.length === 0) {
      return <div>Нет фактов для отображения</div>;
   }

   return (
      <div className="factsContainer">
         {facts.map((el, index) => (
            <div className="blockFact" key={index}>
               <p className="factText">{el.fact}</p>
               <p className="factLingthText">Длина текста факта: {el.length}</p>
            </div>
         ))}
      </div>
   );
};

export default FactsList;