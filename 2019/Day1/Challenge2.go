package main

import (
	"bufio"
	"fmt"
	"log"
	"os"
	"strconv"
)

func main() {
	f, err := os.Open("input.txt")
	if err != nil {
		log.Fatalf("failed opening file: %s", err)
	}

	scanner := bufio.NewScanner(f)
	scanner.Split(bufio.ScanLines)
	var txtlines []string

	for scanner.Scan() {
		txtlines = append(txtlines, scanner.Text())
	}

	f.Close()


	var count int64

	for _, eachline := range txtlines {
		i, _ := strconv.ParseInt(eachline, 10, 32)
		count += calcFuelCost(i)
	}

	fmt.Println(count)
}

func calcFuelCost(mass int64) int64 {
	m := (mass / 3) - 2
	if m <= 0 {
		return 0
	}
	return m + calcFuelCost(m)
}
