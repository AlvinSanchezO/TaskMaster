const Calculator = require ('./Calculator');

describe('Calculator', () => {
    let calculator;

    beforeEach(() => {
        calculator = new Calculator();
    });

    test('add two numbers', () => {
        expect(calculator.add(2, 3)).toBe(5);
    });

    test('subtract two numbers', () => {
        expect(calculator.substract(5, 2)).toBe(3);
    });

    test('throw error when dividing by zero', () => {
        expect(() => calculator.divide(5, 0)).toThrow("Cannot divide by zero");
    });
});